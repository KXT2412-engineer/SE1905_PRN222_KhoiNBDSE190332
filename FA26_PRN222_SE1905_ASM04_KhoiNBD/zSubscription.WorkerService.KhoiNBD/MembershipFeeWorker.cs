using Microsoft.Extensions.Options;
using System.Text.Json;
using zSubscription.Services.KhoiNBD;

namespace zSubscription.WorkerService.KhoiNBD;

/// <summary>
/// Background job định kỳ:
///   1. Chuyển các khoản phí đã quá EndDate nhưng còn IsActive sang Inactive.
///   2. Ghi log thống kê doanh thu / số lượng khoản phí.
/// </summary>
public class MembershipFeeWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<WorkerOptions> _options;
    private readonly ILogger<MembershipFeeWorker> _logger;
    private readonly IHostEnvironment _environment;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public MembershipFeeWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<WorkerOptions> options,
        ILogger<MembershipFeeWorker> logger,
        IHostEnvironment environment)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
        _environment = environment;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalSeconds = Math.Max(5, _options.Value.IntervalSeconds);
        _logger.LogInformation(
            "MembershipFeeWorker started. Interval = {Interval}s, DeactivateExpiredFees = {Deactivate}.",
            intervalSeconds,
            _options.Value.DeactivateExpiredFees);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MembershipFeeWorker gặp lỗi trong chu kỳ xử lý.");
                await TryWriteFailureLogAsync(ex, stoppingToken);
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("MembershipFeeWorker stopped.");
    }

    private async Task RunOnceAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var maintenance = scope.ServiceProvider.GetRequiredService<IMembershipFeeMaintenanceService>();

        var deactivated = 0;
        if (_options.Value.DeactivateExpiredFees)
        {
            deactivated = await maintenance.DeactivateExpiredAsync(DateTime.Now);
        }

        var summary = await maintenance.GetSummaryAsync();
        var runAt = DateTime.Now;

        _logger.LogInformation(
            "Run at {RunAt:dd/MM/yyyy HH:mm:ss} | Deactivated: {Deactivated} | Total: {Total} | Active: {Active} | Inactive: {Inactive} | Expired: {Expired} | Revenue: {Revenue:N0} VND | This month: {RevenueThisMonth:N0} VND | Packages: {Packages}",
            runAt,
            deactivated,
            summary.TotalFees,
            summary.ActiveFees,
            summary.InactiveFees,
            summary.ExpiredFees,
            summary.TotalRevenue,
            summary.RevenueThisMonth,
            summary.PackageCount);

        if (_options.Value.WriteFileLog)
        {
            await WriteFileLogAsync(
                new WorkerLogEntry(
                    runAt,
                    "Success",
                    null,
                    deactivated,
                    summary.TotalFees,
                    summary.ActiveFees,
                    summary.InactiveFees,
                    summary.ExpiredFees,
                    summary.TotalRevenue,
                    summary.RevenueThisMonth,
                    summary.PackageCount),
                cancellationToken);
        }

        if (deactivated > 0)
        {
            _logger.LogWarning(
                "Đã tự động chuyển {Count} khoản phí quá hạn sang trạng thái Inactive.",
                deactivated);
        }
    }

    private async Task WriteFileLogAsync(WorkerLogEntry entry, CancellationToken cancellationToken)
    {
        var configuredPath = _options.Value.LogFilePath;
        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            configuredPath = Path.Combine("Logs", "membership-worker.jsonl");
        }

        var fullPath = Path.IsPathRooted(configuredPath)
            ? Path.GetFullPath(configuredPath)
            : Path.GetFullPath(configuredPath, _environment.ContentRootPath);

        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(entry, JsonOptions);
        await File.AppendAllTextAsync(fullPath, json + Environment.NewLine, cancellationToken);
    }

    private async Task TryWriteFailureLogAsync(Exception exception, CancellationToken cancellationToken)
    {
        if (!_options.Value.WriteFileLog)
        {
            return;
        }

        try
        {
            await WriteFileLogAsync(
                new WorkerLogEntry(
                    DateTime.Now,
                    "Error",
                    exception.Message,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null),
                cancellationToken);
        }
        catch (Exception logException)
        {
            _logger.LogError(logException, "Không thể ghi file log của Worker Service.");
        }
    }

    private sealed record WorkerLogEntry(
        DateTime RunAt,
        string Status,
        string? Error,
        int? Deactivated,
        int? TotalFees,
        int? ActiveFees,
        int? InactiveFees,
        int? ExpiredFees,
        decimal? TotalRevenue,
        decimal? RevenueThisMonth,
        int? PackageCount);
}

using zSubscription.Services.KhoiNBD;
using Microsoft.Extensions.Options;

namespace zSubscription.WorkerService.KhoiNBD;

public sealed class MembershipFeeWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly MembershipFeeSnapshotWriter _writer;
    private readonly ILogger<MembershipFeeWorker> _logger;
    private readonly WorkerOptions _options;

    public MembershipFeeWorker(
        IServiceScopeFactory scopeFactory,
        MembershipFeeSnapshotWriter writer,
        IOptions<WorkerOptions> options,
        ILogger<MembershipFeeWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _writer = writer;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Membership Fee worker started. Interval: {Interval}. Text file: {FilePath}",
            TimeSpan.FromSeconds(Math.Max(1, _options.IntervalSeconds)),
            _writer.FilePath);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var maintenance = scope.ServiceProvider.GetRequiredService<IMembershipFeeMaintenanceService>();
                    var deactivatedCount = _options.DeactivateExpiredFees
                        ? await maintenance.DeactivateExpiredAsync(DateTime.Now)
                        : 0;
                    var service = scope.ServiceProvider.GetRequiredService<IMembershipFeeKhoiNbdService>();
                    var items = await service.GetAllAsync();
                    if (_writer.IsEnabled)
                    {
                        await _writer.WriteAsync(items, stoppingToken);
                    }

                    _logger.LogInformation(
                        "Processed {Count} Membership Fees; deactivated {DeactivatedCount} expired fees. Snapshot: {FilePath} at {Time}",
                        items.Count,
                        deactivatedCount,
                        _writer.FilePath,
                        DateTimeOffset.Now);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Worker cycle failed. Retrying after {IntervalSeconds} seconds.",
                        _options.IntervalSeconds);
                }

                await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, _options.IntervalSeconds)), stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }

        _logger.LogInformation("MembershipFeeWorker stopped.");
    }
}

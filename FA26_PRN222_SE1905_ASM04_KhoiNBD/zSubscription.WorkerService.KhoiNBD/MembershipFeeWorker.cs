using zSubscription.Services.KhoiNBD;

namespace zSubscription.WorkerService.KhoiNBD;

public sealed class MembershipFeeWorker : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly MembershipFeeSnapshotWriter _writer;
    private readonly ILogger<MembershipFeeWorker> _logger;

    public MembershipFeeWorker(
        IServiceScopeFactory scopeFactory,
        MembershipFeeSnapshotWriter writer,
        ILogger<MembershipFeeWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _writer = writer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Membership Fee worker started. Interval: {Interval}. Text file: {FilePath}",
            Interval,
            _writer.FilePath);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IMembershipFeeKhoiNbdService>();
                    var items = await service.GetAllAsync();
                    await _writer.WriteAsync(items, stoppingToken);

                    _logger.LogInformation(
                        "Appended {Count} Membership Fees to {FilePath} at {Time}",
                        items.Count,
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
                        "Could not read Membership Fees or append the text file. Retrying in 5 seconds.");
                }

                await Task.Delay(Interval, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }

        _logger.LogInformation("MembershipFeeWorker stopped.");
    }
}

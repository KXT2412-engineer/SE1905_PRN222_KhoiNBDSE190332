namespace zSubscription.WorkerService.KhoiNBD;

public class WorkerOptions
{
    public const string SectionName = "Worker";

    public int IntervalSeconds { get; set; } = 5;
    public string FilePath { get; set; } = Path.Combine("Logs", "KhoiNBD_SE1905_WS.txt");
}

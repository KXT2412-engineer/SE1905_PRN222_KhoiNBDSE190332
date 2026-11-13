namespace zSubscription.WorkerService.KhoiNBD;

public class WorkerOptions
{
    public const string SectionName = "Worker";

    public int IntervalSeconds { get; set; } = 5;
    public bool DeactivateExpiredFees { get; set; }
    public bool WriteFileLog { get; set; } = true;
    public string LogFilePath { get; set; } = Path.Combine("Logs", "KhoiNBD_SE1905_WS.txt");
}

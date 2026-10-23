namespace zSubscription.WorkerService.KhoiNBD;

public class WorkerOptions
{
    public const string SectionName = "Worker";

    /// <summary>Chu kỳ chạy job (giây).</summary>
    public int IntervalSeconds { get; set; } = 30;

    /// <summary>Tự động chuyển các khoản phí quá hạn sang Inactive.</summary>
    public bool DeactivateExpiredFees { get; set; } = true;

    /// <summary>Ghi kết quả mỗi chu kỳ ra file JSON Lines.</summary>
    public bool WriteFileLog { get; set; } = true;

    /// <summary>
    /// Đường dẫn file log. Đường dẫn tương đối được tính từ thư mục chạy ứng dụng.
    /// </summary>
    public string LogFilePath { get; set; } = Path.Combine("Logs", "membership-worker.jsonl");
}

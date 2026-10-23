namespace zSubscription.TCPServer.KhoiNBD.Models;

/// <summary>
/// Dữ liệu truyền qua TCP, bám theo các cột có thật trong database PRN222 của KhoiNBD.
/// ASM05 dùng dữ liệu mẫu trong bộ nhớ và không thay đổi database.
/// </summary>
public sealed class MembershipFeeKhoiNbdRecord
{
    public int MembershipFeeKhoiNbdid { get; init; }
    public string MemberName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime PaymentDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Notes { get; init; }
    public int? Status { get; init; }
    public DateTime? PublishDate { get; init; }
    public int? FeePackageKhoiNbdid { get; init; }
    public string? PackageName { get; init; }
    public bool IsActive { get; init; }
}

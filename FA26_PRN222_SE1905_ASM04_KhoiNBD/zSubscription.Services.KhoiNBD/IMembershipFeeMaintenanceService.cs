using zSubscription.Entities.KhoiNBD.Models;

namespace zSubscription.Services.KhoiNBD;

/// <summary>
/// Nghiệp vụ nền dùng cho Worker Service (ASM4).
/// </summary>
public interface IMembershipFeeMaintenanceService
{
    Task<int> DeactivateExpiredAsync(DateTime asOf);
    Task<MembershipFeeSummary> GetSummaryAsync();
}

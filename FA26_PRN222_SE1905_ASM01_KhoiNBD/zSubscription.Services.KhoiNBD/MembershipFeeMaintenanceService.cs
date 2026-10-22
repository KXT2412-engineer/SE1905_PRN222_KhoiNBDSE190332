using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD;

namespace zSubscription.Services.KhoiNBD;

public class MembershipFeeMaintenanceService : IMembershipFeeMaintenanceService
{
    private readonly MembershipFeeKhoiNbdRepository _repository;

    public MembershipFeeMaintenanceService(MembershipFeeKhoiNbdRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> DeactivateExpiredAsync(DateTime asOf)
    {
        var expiredItems = await _repository.GetExpiredActiveAsync(asOf);
        if (expiredItems.Count == 0)
        {
            return 0;
        }

        foreach (var item in expiredItems)
        {
            item.IsActive = false;
            item.Status = 0; // 0 = hết hạn
            item.UpdateAt = DateTime.Now;
        }

        // Các entity đang được tracking bởi GetExpiredActiveAsync (.AsTracking()).
        await _repository.UpdateRangeAsync(expiredItems);
        return expiredItems.Count;
    }

    public Task<MembershipFeeSummary> GetSummaryAsync()
        => _repository.GetSummaryAsync();
}

using zSubscription.Entities.KhoiNBD.Models;

namespace zSubscription.Services.KhoiNBD;

public interface IMembershipFeeKhoiNbdService
{
    Task<List<MembershipFeeKhoiNbd>> GetAllAsync();
    Task<MembershipFeeKhoiNbd?> GetByIdAsync(int id);
    Task<List<MembershipFeeKhoiNbd>> SearchAsync(string? memberName, string? activeStatus, string? packageName);
    Task<MembershipFeeKhoiNbd> CreateAsync(MembershipFeeKhoiNbd membershipFee);
    Task<bool> UpdateAsync(MembershipFeeKhoiNbd membershipFee);
    Task<bool> DeleteAsync(int id);
}

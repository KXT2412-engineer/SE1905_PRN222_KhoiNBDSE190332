using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD;

namespace zSubscription.Services.KhoiNBD;

public class MembershipFeeKhoiNbdService : IMembershipFeeKhoiNbdService
{
    private readonly MembershipFeeKhoiNbdRepository _repository;

    public MembershipFeeKhoiNbdService(MembershipFeeKhoiNbdRepository repository)
    {
        _repository = repository;
    }

    public Task<List<MembershipFeeKhoiNbd>> GetAllAsync()
        => _repository.GetAllAsync();

    public Task<MembershipFeeKhoiNbd?> GetByIdAsync(int id)
        => _repository.GetByIdAsync(id);

    public Task<List<MembershipFeeKhoiNbd>> SearchAsync(string? memberName, string? activeStatus, string? packageName)
        => _repository.SearchAsync(memberName, activeStatus, packageName);

    public async Task<MembershipFeeKhoiNbd> CreateAsync(MembershipFeeKhoiNbd membershipFee)
    {
        ArgumentNullException.ThrowIfNull(membershipFee);

        // Không đính kèm navigation khi thêm mới, tránh EF cố insert cả FeePackage.
        membershipFee.FeePackageKhoiNbd = null;
        membershipFee.MembershipFeeKhoiNbdid = 0;
        membershipFee.PaymentDate = membershipFee.PaymentDate == default
            ? DateTime.Today
            : membershipFee.PaymentDate;
        membershipFee.PublishDate ??= DateTime.Today;
        membershipFee.UpdateAt = DateTime.Now;

        return await _repository.CreateAsync(membershipFee);
    }

    public async Task<bool> UpdateAsync(MembershipFeeKhoiNbd membershipFee)
    {
        ArgumentNullException.ThrowIfNull(membershipFee);

        var existing = await _repository.GetByIdAsync(membershipFee.MembershipFeeKhoiNbdid);
        if (existing is null)
        {
            return false;
        }

        // Cập nhật trực tiếp trên entity đang được DbContext theo dõi để tránh lỗi
        // "another instance with the same key value is already being tracked".
        // Đặc biệt quan trọng với Blazor Server vì DbContext sống theo circuit.
        existing.MemberName = membershipFee.MemberName;
        existing.Amount = membershipFee.Amount;
        existing.PaymentDate = membershipFee.PaymentDate;
        existing.EndDate = membershipFee.EndDate;
        existing.Notes = membershipFee.Notes;
        existing.Status = membershipFee.Status;
        existing.PublishDate = membershipFee.PublishDate ?? existing.PublishDate ?? DateTime.Today;
        existing.FeePackageKhoiNbdid = membershipFee.FeePackageKhoiNbdid;
        existing.IsActive = membershipFee.IsActive;
        existing.UpdateAt = DateTime.Now;

        // Không đính kèm navigation để tránh EF cập nhật lan sang FeePackage.
        existing.FeePackageKhoiNbd = null;

        return await _repository.UpdateAsync(existing);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
        {
            return false;
        }

        return await _repository.DeleteAsync(existing);
    }
}

using Microsoft.EntityFrameworkCore;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD.Base;
using zSubscription.Repositories.KhoiNBD.DBContext;

namespace zSubscription.Repositories.KhoiNBD;

public class MembershipFeeKhoiNbdRepository : GenericRepository<MembershipFeeKhoiNbd>
{
    public MembershipFeeKhoiNbdRepository(PRN222Context context) : base(context)
    {
    }

    public override async Task<List<MembershipFeeKhoiNbd>> GetAllAsync()
    {
        return await _context.MembershipFeeKhoiNbds
            .Include(x => x.FeePackageKhoiNbd)
            .OrderByDescending(x => x.MembershipFeeKhoiNbdid)
            .ToListAsync();
    }

    public override async Task<MembershipFeeKhoiNbd?> GetByIdAsync(int id)
    {
        return await _context.MembershipFeeKhoiNbds
            .Include(x => x.FeePackageKhoiNbd)
            .FirstOrDefaultAsync(x => x.MembershipFeeKhoiNbdid == id);
    }

    /// <summary>
    /// Tìm kiếm theo 3 tiêu chí có thật trong DB của KhoiNBD:
    /// tên hội viên, trạng thái hoạt động và tên gói.
    /// </summary>
    public async Task<List<MembershipFeeKhoiNbd>> SearchAsync(
        string? memberName,
        string? activeStatus,
        string? packageName)
    {
        var query = _context.MembershipFeeKhoiNbds
            .Include(x => x.FeePackageKhoiNbd)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(memberName))
        {
            var keyword = memberName.Trim();
            query = query.Where(x => x.MemberName.Contains(keyword));
        }

        if (bool.TryParse(activeStatus, out var isActive))
        {
            query = query.Where(x => x.IsActive == isActive);
        }

        if (!string.IsNullOrWhiteSpace(packageName))
        {
            var keyword = packageName.Trim();
            query = query.Where(x =>
                x.FeePackageKhoiNbd != null && x.FeePackageKhoiNbd.PackageName.Contains(keyword));
        }

        return await query
            .OrderByDescending(x => x.MembershipFeeKhoiNbdid)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy các khoản phí còn Active nhưng đã quá EndDate (dùng cho Worker).
    /// </summary>
    public async Task<List<MembershipFeeKhoiNbd>> GetExpiredActiveAsync(DateTime asOf)
    {
        return await _context.MembershipFeeKhoiNbds
            .AsTracking()
            .Where(x => x.IsActive && x.EndDate != null && x.EndDate < asOf)
            .ToListAsync();
    }

    public async Task<MembershipFeeSummary> GetSummaryAsync()
    {
        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

        return new MembershipFeeSummary
        {
            TotalFees = await _context.MembershipFeeKhoiNbds.CountAsync(),
            ActiveFees = await _context.MembershipFeeKhoiNbds.CountAsync(x => x.IsActive),
            InactiveFees = await _context.MembershipFeeKhoiNbds.CountAsync(x => !x.IsActive),
            ExpiredFees = await _context.MembershipFeeKhoiNbds
                .CountAsync(x => x.EndDate != null && x.EndDate < today),
            TotalRevenue = await _context.MembershipFeeKhoiNbds
                .SumAsync(x => (decimal?)x.Amount) ?? 0m,
            RevenueThisMonth = await _context.MembershipFeeKhoiNbds
                .Where(x => x.PaymentDate >= firstDayOfMonth)
                .SumAsync(x => (decimal?)x.Amount) ?? 0m,
            PackageCount = await _context.FeePackageKhoiNbds.CountAsync()
        };
    }
}

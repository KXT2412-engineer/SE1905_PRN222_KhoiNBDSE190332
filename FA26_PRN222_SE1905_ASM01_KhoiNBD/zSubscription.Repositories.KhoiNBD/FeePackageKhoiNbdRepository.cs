using Microsoft.EntityFrameworkCore;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD.Base;
using zSubscription.Repositories.KhoiNBD.DBContext;

namespace zSubscription.Repositories.KhoiNBD;

public class FeePackageKhoiNbdRepository : GenericRepository<FeePackageKhoiNbd>
{
    public FeePackageKhoiNbdRepository(PRN222Context context) : base(context)
    {
    }

    public override async Task<List<FeePackageKhoiNbd>> GetAllAsync()
    {
        return await _context.FeePackageKhoiNbds
            .OrderBy(x => x.PackageName)
            .ToListAsync();
    }
}

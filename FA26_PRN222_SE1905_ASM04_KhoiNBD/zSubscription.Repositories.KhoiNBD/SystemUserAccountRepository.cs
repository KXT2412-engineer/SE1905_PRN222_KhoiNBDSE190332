using Microsoft.EntityFrameworkCore;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD.Base;
using zSubscription.Repositories.KhoiNBD.DBContext;

namespace zSubscription.Repositories.KhoiNBD;

public class SystemUserAccountRepository : GenericRepository<SystemUserAccount>
{
    public SystemUserAccountRepository(PRN222Context context) : base(context)
    {
    }

    public async Task<SystemUserAccount?> GetUserAccountAsync(string userName, string password)
    {
        return await _context.SystemUserAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.UserName == userName &&
                x.Password == password &&
                x.IsActive);
    }
}

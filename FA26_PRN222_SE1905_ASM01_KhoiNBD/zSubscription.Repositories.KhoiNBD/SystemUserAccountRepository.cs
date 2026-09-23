using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD.DBContext;

namespace zSubscription.Repositories.KhoiNBD
{
    public class SystemUserAccountRepository : GenericRepository<SystemUserAccount>
    {
        public SystemUserAccountRepository() => _context ??= new PRN222Context();
        public SystemUserAccountRepository(PRN222Context context) => _context = context;
        public async Task<SystemUserAccount> GetByUserNameAsync(string userName, string password)
        {
            return await _context.SystemUserAccounts.FirstOrDefaultAsync(u => u.Email == userName && u.Password == password && u.IsActive);
        }
    }
}

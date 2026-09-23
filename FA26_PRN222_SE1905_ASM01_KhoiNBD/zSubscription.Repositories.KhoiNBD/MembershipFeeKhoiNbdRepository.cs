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
    public class MembershipFeeKhoiNbdRepository : GenericRepository<MembershipFeeKhoiNbd>
    {
        public MembershipFeeKhoiNbdRepository() => _context ??= new PRN222Context();
        public MembershipFeeKhoiNbdRepository(PRN222Context context) => _context = context;

        public new async Task<List<MembershipFeeKhoiNbd>> GetAllAsync()
        {
            return await _context.MembershipFeeKhoiNbds.Include(c => c.FeePackageKhoiNbd).ToListAsync();
        }
        public new async Task<MembershipFeeKhoiNbd> GetByIdAsync(int id)
        {
            return await _context.MembershipFeeKhoiNbds.Include(c => c.FeePackageKhoiNbd).FirstOrDefaultAsync(c => c.MembershipFeeKhoiNbdid == id);
        }
        public async Task<List<MembershipFeeKhoiNbd>> SearchAsync(decimal? amount, string? transCode, string? payMethod)
        {
            return await _context.MembershipFeeKhoiNbds
                .Include(c => c.FeePackageKhoiNbd)
                .Where(c =>
                    (!amount.HasValue || c.Amount == amount.Value) &&
                    (string.IsNullOrEmpty(transCode) || (c.TransactionCode != null && c.TransactionCode.Contains(transCode))) &&
                    (string.IsNullOrEmpty(payMethod) || (c.PaymentMethod != null && c.PaymentMethod.Contains(payMethod))))
                .ToListAsync();
        }
    }
}

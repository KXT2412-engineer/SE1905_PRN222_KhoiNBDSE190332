using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD.DBContext;

namespace zSubscription.Repositories.KhoiNBD
{
    public class FeePackageKhoiNbdRepository : GenericRepository<FeePackageKhoiNbd>
    {
        public FeePackageKhoiNbdRepository() => _context ??= new PRN222Context();
        public FeePackageKhoiNbdRepository(PRN222Context context) => _context = context;
    }
}

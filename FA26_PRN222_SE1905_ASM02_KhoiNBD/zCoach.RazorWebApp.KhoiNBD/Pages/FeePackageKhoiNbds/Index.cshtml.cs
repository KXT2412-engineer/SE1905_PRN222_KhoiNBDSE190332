using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD.DBContext;

namespace zCoach.RazorWebApp.KhoiNBD.Pages_FeePackageKhoiNbds
{
    public class IndexModel : PageModel
    {
        private readonly zSubscription.Repositories.KhoiNBD.DBContext.PRN222Context _context;

        public IndexModel(zSubscription.Repositories.KhoiNBD.DBContext.PRN222Context context)
        {
            _context = context;
        }

        public IList<FeePackageKhoiNbd> FeePackageKhoiNbd { get;set; } = default!;

        public async Task OnGetAsync()
        {
            FeePackageKhoiNbd = await _context.FeePackageKhoiNbds.ToListAsync();
        }
    }
}

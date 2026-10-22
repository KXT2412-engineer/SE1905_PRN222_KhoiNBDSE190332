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
    public class DetailsModel : PageModel
    {
        private readonly zSubscription.Repositories.KhoiNBD.DBContext.PRN222Context _context;

        public DetailsModel(zSubscription.Repositories.KhoiNBD.DBContext.PRN222Context context)
        {
            _context = context;
        }

        public FeePackageKhoiNbd FeePackageKhoiNbd { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feepackagekhoinbd = await _context.FeePackageKhoiNbds.FirstOrDefaultAsync(m => m.FeePackageKhoiNbdid == id);

            if (feepackagekhoinbd is not null)
            {
                FeePackageKhoiNbd = feepackagekhoinbd;

                return Page();
            }

            return NotFound();
        }
    }
}

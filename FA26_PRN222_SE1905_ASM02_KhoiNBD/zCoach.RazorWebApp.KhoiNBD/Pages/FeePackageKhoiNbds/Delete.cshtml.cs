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
    public class DeleteModel : PageModel
    {
        private readonly zSubscription.Repositories.KhoiNBD.DBContext.PRN222Context _context;

        public DeleteModel(zSubscription.Repositories.KhoiNBD.DBContext.PRN222Context context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feepackagekhoinbd = await _context.FeePackageKhoiNbds.FindAsync(id);
            if (feepackagekhoinbd != null)
            {
                FeePackageKhoiNbd = feepackagekhoinbd;
                _context.FeePackageKhoiNbds.Remove(FeePackageKhoiNbd);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

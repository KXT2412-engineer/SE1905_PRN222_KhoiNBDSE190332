using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD.DBContext;

namespace zCoach.RazorWebApp.KhoiNBD.Pages_FeePackageKhoiNbds
{
    public class EditModel : PageModel
    {
        private readonly zSubscription.Repositories.KhoiNBD.DBContext.PRN222Context _context;

        public EditModel(zSubscription.Repositories.KhoiNBD.DBContext.PRN222Context context)
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

            var feepackagekhoinbd =  await _context.FeePackageKhoiNbds.FirstOrDefaultAsync(m => m.FeePackageKhoiNbdid == id);
            if (feepackagekhoinbd == null)
            {
                return NotFound();
            }
            FeePackageKhoiNbd = feepackagekhoinbd;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(FeePackageKhoiNbd).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeePackageKhoiNbdExists(FeePackageKhoiNbd.FeePackageKhoiNbdid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool FeePackageKhoiNbdExists(int id)
        {
            return _context.FeePackageKhoiNbds.Any(e => e.FeePackageKhoiNbdid == id);
        }
    }
}

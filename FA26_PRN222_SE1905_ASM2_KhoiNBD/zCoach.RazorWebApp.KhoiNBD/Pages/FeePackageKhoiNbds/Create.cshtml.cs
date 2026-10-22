using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD.DBContext;

namespace zCoach.RazorWebApp.KhoiNBD.Pages_FeePackageKhoiNbds
{
    public class CreateModel : PageModel
    {
        private readonly zSubscription.Repositories.KhoiNBD.DBContext.PRN222Context _context;

        public CreateModel(zSubscription.Repositories.KhoiNBD.DBContext.PRN222Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public FeePackageKhoiNbd FeePackageKhoiNbd { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.FeePackageKhoiNbds.Add(FeePackageKhoiNbd);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

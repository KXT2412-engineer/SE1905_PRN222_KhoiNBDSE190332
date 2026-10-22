using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD.DBContext;

namespace zSubscription.MVCWebApp.KhoiNBD.Controllers
{
    public class MembershipFeeKhoiNbdController : Controller
    {
        private readonly PRN222Context _context;
        private const int PageSize = 5;

        public MembershipFeeKhoiNbdController(PRN222Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchName, int? searchStatus, bool? isActive, int page = 1)
        {
            var query = _context.MembershipFeeKhoiNbds
                .Include(m => m.FeePackageKhoiNbd)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
                query = query.Where(m => m.MemberName.Contains(searchName));

            if (searchStatus.HasValue)
                query = query.Where(m => m.Status == searchStatus);

            if (isActive.HasValue)
                query = query.Where(m => m.IsActive == isActive);

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var items = await query
                .OrderBy(m => m.MembershipFeeKhoiNbdid)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.SearchName = searchName;
            ViewBag.SearchStatus = searchStatus;
            ViewBag.IsActive = isActive;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.MembershipFeeKhoiNbds
                .Include(m => m.FeePackageKhoiNbd)
                .FirstOrDefaultAsync(m => m.MembershipFeeKhoiNbdid == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create()
        {
            ViewData["FeePackageKhoiNbdid"] = new SelectList(_context.FeePackageKhoiNbds, "FeePackageKhoiNbdid", "PackageName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembershipFeeKhoiNbdid,MemberName,Amount,PaymentDate,EndDate,Notes,Status,PublishDate,UpdateAt,FeePackageKhoiNbdid,IsActive")] MembershipFeeKhoiNbd membershipFeeKhoiNbd)
        {
            if (ModelState.IsValid)
            {
                _context.Add(membershipFeeKhoiNbd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FeePackageKhoiNbdid"] = new SelectList(_context.FeePackageKhoiNbds, "FeePackageKhoiNbdid", "PackageName", membershipFeeKhoiNbd.FeePackageKhoiNbdid);
            return View(membershipFeeKhoiNbd);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.MembershipFeeKhoiNbds.FindAsync(id);
            if (item == null) return NotFound();
            ViewData["FeePackageKhoiNbdid"] = new SelectList(_context.FeePackageKhoiNbds, "FeePackageKhoiNbdid", "PackageName", item.FeePackageKhoiNbdid);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MembershipFeeKhoiNbdid,MemberName,Amount,PaymentDate,EndDate,Notes,Status,PublishDate,UpdateAt,FeePackageKhoiNbdid,IsActive")] MembershipFeeKhoiNbd membershipFeeKhoiNbd)
        {
            if (id != membershipFeeKhoiNbd.MembershipFeeKhoiNbdid) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(membershipFeeKhoiNbd); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.MembershipFeeKhoiNbds.Any(e => e.MembershipFeeKhoiNbdid == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FeePackageKhoiNbdid"] = new SelectList(_context.FeePackageKhoiNbds, "FeePackageKhoiNbdid", "PackageName", membershipFeeKhoiNbd.FeePackageKhoiNbdid);
            return View(membershipFeeKhoiNbd);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.MembershipFeeKhoiNbds
                .Include(m => m.FeePackageKhoiNbd)
                .FirstOrDefaultAsync(m => m.MembershipFeeKhoiNbdid == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.MembershipFeeKhoiNbds.FindAsync(id);
            if (item != null) _context.MembershipFeeKhoiNbds.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

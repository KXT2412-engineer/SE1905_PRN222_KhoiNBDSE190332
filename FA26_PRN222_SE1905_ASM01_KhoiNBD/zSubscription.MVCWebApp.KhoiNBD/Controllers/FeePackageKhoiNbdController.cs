using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD.DBContext;

namespace zSubscription.MVCWebApp.KhoiNBD.Controllers
{
    public class FeePackageKhoiNbdController : Controller
    {
        private readonly PRN222Context _context;
        private const int PageSize = 5;

        public FeePackageKhoiNbdController(PRN222Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchName, int? searchStatus, decimal? maxPrice, int page = 1)
        {
            var query = _context.FeePackageKhoiNbds.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
                query = query.Where(f => f.PackageName.Contains(searchName));

            if (searchStatus.HasValue)
                query = query.Where(f => f.Status == searchStatus);

            if (maxPrice.HasValue)
                query = query.Where(f => f.Price <= maxPrice);

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var items = await query
                .OrderBy(f => f.FeePackageKhoiNbdid)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.SearchName = searchName;
            ViewBag.SearchStatus = searchStatus;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.FeePackageKhoiNbds.FirstOrDefaultAsync(m => m.FeePackageKhoiNbdid == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FeePackageKhoiNbdid,PackageName,Description,DurationInDays,Price,Status,PublishDate")] FeePackageKhoiNbd feePackageKhoiNbd)
        {
            if (ModelState.IsValid)
            {
                _context.Add(feePackageKhoiNbd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feePackageKhoiNbd);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.FeePackageKhoiNbds.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FeePackageKhoiNbdid,PackageName,Description,DurationInDays,Price,Status,PublishDate")] FeePackageKhoiNbd feePackageKhoiNbd)
        {
            if (id != feePackageKhoiNbd.FeePackageKhoiNbdid) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(feePackageKhoiNbd); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.FeePackageKhoiNbds.Any(e => e.FeePackageKhoiNbdid == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(feePackageKhoiNbd);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.FeePackageKhoiNbds.FirstOrDefaultAsync(m => m.FeePackageKhoiNbdid == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.FeePackageKhoiNbds.FindAsync(id);
            if (item != null) _context.FeePackageKhoiNbds.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

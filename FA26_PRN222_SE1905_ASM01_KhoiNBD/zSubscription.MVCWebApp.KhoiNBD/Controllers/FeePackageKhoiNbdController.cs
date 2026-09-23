using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD.DBContext;

namespace zSubscription.MVCWebApp.KhoiNBD.Controllers
{
    public class FeePackageKhoiNbdController : Controller
    {
        private readonly PRN222Context _context;

        public FeePackageKhoiNbdController(PRN222Context context)
        {
            _context = context;
        }

        // GET: FeePackageKhoiNbd
        public async Task<IActionResult> Index()
        {
            return View(await _context.FeePackageKhoiNbds.ToListAsync());
        }

        // GET: FeePackageKhoiNbd/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feePackageKhoiNbd = await _context.FeePackageKhoiNbds
                .FirstOrDefaultAsync(m => m.FeePackageKhoiNbdid == id);
            if (feePackageKhoiNbd == null)
            {
                return NotFound();
            }

            return View(feePackageKhoiNbd);
        }

        // GET: FeePackageKhoiNbd/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FeePackageKhoiNbd/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: FeePackageKhoiNbd/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feePackageKhoiNbd = await _context.FeePackageKhoiNbds.FindAsync(id);
            if (feePackageKhoiNbd == null)
            {
                return NotFound();
            }
            return View(feePackageKhoiNbd);
        }

        // POST: FeePackageKhoiNbd/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FeePackageKhoiNbdid,PackageName,Description,DurationInDays,Price,Status,PublishDate")] FeePackageKhoiNbd feePackageKhoiNbd)
        {
            if (id != feePackageKhoiNbd.FeePackageKhoiNbdid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feePackageKhoiNbd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeePackageKhoiNbdExists(feePackageKhoiNbd.FeePackageKhoiNbdid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(feePackageKhoiNbd);
        }

        // GET: FeePackageKhoiNbd/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feePackageKhoiNbd = await _context.FeePackageKhoiNbds
                .FirstOrDefaultAsync(m => m.FeePackageKhoiNbdid == id);
            if (feePackageKhoiNbd == null)
            {
                return NotFound();
            }

            return View(feePackageKhoiNbd);
        }

        // POST: FeePackageKhoiNbd/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feePackageKhoiNbd = await _context.FeePackageKhoiNbds.FindAsync(id);
            if (feePackageKhoiNbd != null)
            {
                _context.FeePackageKhoiNbds.Remove(feePackageKhoiNbd);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeePackageKhoiNbdExists(int id)
        {
            return _context.FeePackageKhoiNbds.Any(e => e.FeePackageKhoiNbdid == id);
        }
    }
}

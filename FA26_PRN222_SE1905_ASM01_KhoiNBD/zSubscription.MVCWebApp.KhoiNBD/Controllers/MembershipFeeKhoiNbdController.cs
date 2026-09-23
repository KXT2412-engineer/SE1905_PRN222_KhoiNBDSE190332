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
    public class MembershipFeeKhoiNbdController : Controller
    {
        private readonly PRN222Context _context;

        public MembershipFeeKhoiNbdController(PRN222Context context)
        {
            _context = context;
        }

        // GET: MembershipFeeKhoiNbd
        public async Task<IActionResult> Index()
        {
            var pRN222Context = _context.MembershipFeeKhoiNbds.Include(m => m.FeePackageKhoiNbd);
            return View(await pRN222Context.ToListAsync());
        }

        // GET: MembershipFeeKhoiNbd/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipFeeKhoiNbd = await _context.MembershipFeeKhoiNbds
                .Include(m => m.FeePackageKhoiNbd)
                .FirstOrDefaultAsync(m => m.MembershipFeeKhoiNbdid == id);
            if (membershipFeeKhoiNbd == null)
            {
                return NotFound();
            }

            return View(membershipFeeKhoiNbd);
        }

        // GET: MembershipFeeKhoiNbd/Create
        public IActionResult Create()
        {
            ViewData["FeePackageKhoiNbdid"] = new SelectList(_context.FeePackageKhoiNbds, "FeePackageKhoiNbdid", "PackageName");
            return View();
        }

        // POST: MembershipFeeKhoiNbd/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembershipFeeKhoiNbdid,MemberName,Amount,PaymentDate,PaymentMethod,TransactionCode,StartDate,EndDate,Notes,Status,PublishDate,UpdateAt,FeePackageKhoiNbdid,IsActive")] MembershipFeeKhoiNbd membershipFeeKhoiNbd)
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

        // GET: MembershipFeeKhoiNbd/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipFeeKhoiNbd = await _context.MembershipFeeKhoiNbds.FindAsync(id);
            if (membershipFeeKhoiNbd == null)
            {
                return NotFound();
            }
            ViewData["FeePackageKhoiNbdid"] = new SelectList(_context.FeePackageKhoiNbds, "FeePackageKhoiNbdid", "PackageName", membershipFeeKhoiNbd.FeePackageKhoiNbdid);
            return View(membershipFeeKhoiNbd);
        }

        // POST: MembershipFeeKhoiNbd/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MembershipFeeKhoiNbdid,MemberName,Amount,PaymentDate,PaymentMethod,TransactionCode,StartDate,EndDate,Notes,Status,PublishDate,UpdateAt,FeePackageKhoiNbdid,IsActive")] MembershipFeeKhoiNbd membershipFeeKhoiNbd)
        {
            if (id != membershipFeeKhoiNbd.MembershipFeeKhoiNbdid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membershipFeeKhoiNbd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipFeeKhoiNbdExists(membershipFeeKhoiNbd.MembershipFeeKhoiNbdid))
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
            ViewData["FeePackageKhoiNbdid"] = new SelectList(_context.FeePackageKhoiNbds, "FeePackageKhoiNbdid", "PackageName", membershipFeeKhoiNbd.FeePackageKhoiNbdid);
            return View(membershipFeeKhoiNbd);
        }

        // GET: MembershipFeeKhoiNbd/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipFeeKhoiNbd = await _context.MembershipFeeKhoiNbds
                .Include(m => m.FeePackageKhoiNbd)
                .FirstOrDefaultAsync(m => m.MembershipFeeKhoiNbdid == id);
            if (membershipFeeKhoiNbd == null)
            {
                return NotFound();
            }

            return View(membershipFeeKhoiNbd);
        }

        // POST: MembershipFeeKhoiNbd/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membershipFeeKhoiNbd = await _context.MembershipFeeKhoiNbds.FindAsync(id);
            if (membershipFeeKhoiNbd != null)
            {
                _context.MembershipFeeKhoiNbds.Remove(membershipFeeKhoiNbd);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MembershipFeeKhoiNbdExists(int id)
        {
            return _context.MembershipFeeKhoiNbds.Any(e => e.MembershipFeeKhoiNbdid == id);
        }
    }
}

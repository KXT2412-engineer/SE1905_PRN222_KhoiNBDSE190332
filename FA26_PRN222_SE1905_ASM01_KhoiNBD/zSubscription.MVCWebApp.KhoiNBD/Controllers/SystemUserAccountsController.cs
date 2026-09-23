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
    public class SystemUserAccountsController : Controller
    {
        private readonly PRN222Context _context;

        public SystemUserAccountsController(PRN222Context context)
        {
            _context = context;
        }

        // GET: SystemUserAccounts
        public async Task<IActionResult> Index()
        {
            return View(await _context.SystemUserAccounts.ToListAsync());
        }

        // GET: SystemUserAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemUserAccount = await _context.SystemUserAccounts
                .FirstOrDefaultAsync(m => m.UserAccountId == id);
            if (systemUserAccount == null)
            {
                return NotFound();
            }

            return View(systemUserAccount);
        }

        // GET: SystemUserAccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SystemUserAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserAccountId,UserName,Password,FullName,Email,Phone,EmployeeCode,RoleId,RequestCode,CreatedDate,ApplicationCode,CreatedBy,ModifiedDate,ModifiedBy,IsActive")] SystemUserAccount systemUserAccount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(systemUserAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(systemUserAccount);
        }

        // GET: SystemUserAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemUserAccount = await _context.SystemUserAccounts.FindAsync(id);
            if (systemUserAccount == null)
            {
                return NotFound();
            }
            return View(systemUserAccount);
        }

        // POST: SystemUserAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserAccountId,UserName,Password,FullName,Email,Phone,EmployeeCode,RoleId,RequestCode,CreatedDate,ApplicationCode,CreatedBy,ModifiedDate,ModifiedBy,IsActive")] SystemUserAccount systemUserAccount)
        {
            if (id != systemUserAccount.UserAccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(systemUserAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemUserAccountExists(systemUserAccount.UserAccountId))
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
            return View(systemUserAccount);
        }

        // GET: SystemUserAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemUserAccount = await _context.SystemUserAccounts
                .FirstOrDefaultAsync(m => m.UserAccountId == id);
            if (systemUserAccount == null)
            {
                return NotFound();
            }

            return View(systemUserAccount);
        }

        // POST: SystemUserAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var systemUserAccount = await _context.SystemUserAccounts.FindAsync(id);
            if (systemUserAccount != null)
            {
                _context.SystemUserAccounts.Remove(systemUserAccount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemUserAccountExists(int id)
        {
            return _context.SystemUserAccounts.Any(e => e.UserAccountId == id);
        }
    }
}

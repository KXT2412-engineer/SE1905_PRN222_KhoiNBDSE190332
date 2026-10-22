using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Services.KhoiNBD;

namespace zSubscription.MVCWebApp.KhoiNBD.Controllers;

public class MembershipFeesController : Controller
{
    private readonly IMembershipFeeKhoiNbdService _membershipFeeService;
    private readonly IFeePackageKhoiNbdService _feePackageService;

    public MembershipFeesController(
        IMembershipFeeKhoiNbdService membershipFeeService,
        IFeePackageKhoiNbdService feePackageService)
    {
        _membershipFeeService = membershipFeeService;
        _feePackageService = feePackageService;
    }

    // GET: /MembershipFees?memberName=...&activeStatus=...&packageName=...
    public async Task<IActionResult> Index(string? memberName, string? activeStatus, string? packageName)
    {
        ViewData["MemberName"] = memberName;
        ViewData["ActiveStatus"] = activeStatus;
        ViewData["PackageName"] = packageName;

        var items = await _membershipFeeService.SearchAsync(memberName, activeStatus, packageName);
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _membershipFeeService.GetByIdAsync(id);
        return item is null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        await LoadPackagesAsync();
        return View(new MembershipFeeKhoiNbd
        {
            PaymentDate = DateTime.Today,
            PublishDate = DateTime.Today,
            IsActive = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MembershipFeeKhoiNbd membershipFee)
    {
        RemoveNavigationErrors();

        if (!ModelState.IsValid)
        {
            await LoadPackagesAsync(membershipFee.FeePackageKhoiNbdid);
            return View(membershipFee);
        }

        await _membershipFeeService.CreateAsync(membershipFee);
        TempData["Success"] = $"Đã tạo khoản phí cho \"{membershipFee.MemberName}\".";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _membershipFeeService.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        await LoadPackagesAsync(item.FeePackageKhoiNbdid);
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MembershipFeeKhoiNbd membershipFee)
    {
        if (id != membershipFee.MembershipFeeKhoiNbdid)
        {
            return BadRequest();
        }

        RemoveNavigationErrors();

        if (!ModelState.IsValid)
        {
            await LoadPackagesAsync(membershipFee.FeePackageKhoiNbdid);
            return View(membershipFee);
        }

        var updated = await _membershipFeeService.UpdateAsync(membershipFee);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = $"Đã cập nhật khoản phí #{membershipFee.MembershipFeeKhoiNbdid}.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await _membershipFeeService.GetByIdAsync(id);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _membershipFeeService.DeleteAsync(id);
        TempData[deleted ? "Success" : "Error"] = deleted
            ? $"Đã xoá khoản phí #{id}."
            : $"Không tìm thấy khoản phí #{id}.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadPackagesAsync(int? selectedId = null)
    {
        var packages = await _feePackageService.GetAllAsync();
        ViewBag.FeePackages = new SelectList(
            packages,
            nameof(FeePackageKhoiNbd.FeePackageKhoiNbdid),
            nameof(FeePackageKhoiNbd.PackageName),
            selectedId);
    }

    /// <summary>
    /// Bỏ qua validate của navigation property vì form chỉ post khoá ngoại.
    /// </summary>
    private void RemoveNavigationErrors()
    {
        var keys = ModelState.Keys
            .Where(k => k.StartsWith(nameof(MembershipFeeKhoiNbd.FeePackageKhoiNbd), StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in keys)
        {
            ModelState.Remove(key);
        }
    }
}

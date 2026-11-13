using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.MVCWebApp.KhoiNBD.Hubs;
using zSubscription.Services.KhoiNBD;

namespace zSubscription.MVCWebApp.KhoiNBD.Controllers;

public class MembershipFeesController : Controller
{
    private readonly IMembershipFeeKhoiNbdService _membershipFeeService;
    private readonly IFeePackageKhoiNbdService _feePackageService;
    private readonly IHubContext<MembershipFeeHub> _hub;

    public MembershipFeesController(
        IMembershipFeeKhoiNbdService membershipFeeService,
        IFeePackageKhoiNbdService feePackageService,
        IHubContext<MembershipFeeHub> hub)
    {
        _membershipFeeService = membershipFeeService;
        _feePackageService = feePackageService;
        _hub = hub;
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

        var created = await _membershipFeeService.CreateAsync(membershipFee);
        await BroadcastChangedAsync("created", created.MembershipFeeKhoiNbdid);
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

        await BroadcastChangedAsync("updated", membershipFee.MembershipFeeKhoiNbdid);

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
        if (deleted)
        {
            await BroadcastChangedAsync("deleted", id);
        }
        TempData[deleted ? "Success" : "Error"] = deleted
            ? $"Đã xoá khoản phí #{id}."
            : $"Không tìm thấy khoản phí #{id}.";
        return RedirectToAction(nameof(Index));
    }

    private async Task BroadcastChangedAsync(string action, int id)
    {
        var item = action == "deleted" ? null : await _membershipFeeService.GetByIdAsync(id);
        await _hub.Clients.All.SendAsync("MembershipFeeChanged", new
        {
            Action = action,
            MembershipFeeKhoiNbdid = id,
            Item = item is null ? null : new
            {
                item.MemberName,
                item.Amount,
                PaymentDate = item.PaymentDate.ToString("yyyy-MM-dd"),
                PackageName = item.FeePackageKhoiNbd?.PackageName,
                EndDate = item.EndDate?.ToString("yyyy-MM-dd"),
                item.Status,
                item.IsActive,
                PublishDate = item.PublishDate?.ToString("yyyy-MM-dd"),
                UpdateAt = item.UpdateAt?.ToString("yyyy-MM-ddTHH:mm:ss"),
                item.Notes
            }
        });
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

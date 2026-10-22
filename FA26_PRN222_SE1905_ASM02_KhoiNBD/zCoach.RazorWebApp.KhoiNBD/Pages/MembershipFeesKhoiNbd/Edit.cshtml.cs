using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Services.KhoiNBD;

namespace zCoach.RazorWebApp.KhoiNBD.Pages.MembershipFeesKhoiNbd;

[Authorize]
public class EditModel : MembershipFeeFormPageModel
{
    private readonly IMembershipFeeKhoiNbdService _membershipFeeService;
    private readonly IFeePackageKhoiNbdService _feePackageService;

    public EditModel(
        IMembershipFeeKhoiNbdService membershipFeeService,
        IFeePackageKhoiNbdService feePackageService)
    {
        _membershipFeeService = membershipFeeService;
        _feePackageService = feePackageService;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var item = await _membershipFeeService.GetByIdAsync(id.Value);
        if (item is null)
        {
            return NotFound();
        }

        MembershipFee = item;
        await LoadPackagesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        RemoveNavigationErrors();

        if (!ModelState.IsValid)
        {
            await LoadPackagesAsync();
            return Page();
        }

        var updated = await _membershipFeeService.UpdateAsync(MembershipFee);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = $"Đã cập nhật khoản phí #{MembershipFee.MembershipFeeKhoiNbdid}.";
        return RedirectToPage("./Index");
    }

    private async Task LoadPackagesAsync()
    {
        var packages = await _feePackageService.GetAllAsync();
        ViewData["FeePackageKhoiNbdid"] = new SelectList(
            packages,
            nameof(FeePackageKhoiNbd.FeePackageKhoiNbdid),
            nameof(FeePackageKhoiNbd.PackageName),
            MembershipFee.FeePackageKhoiNbdid);
    }

    private void RemoveNavigationErrors()
    {
        var keys = ModelState.Keys
            .Where(k => k.Contains("FeePackageKhoiNbd", StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in keys)
        {
            ModelState.Remove(key);
        }
    }
}

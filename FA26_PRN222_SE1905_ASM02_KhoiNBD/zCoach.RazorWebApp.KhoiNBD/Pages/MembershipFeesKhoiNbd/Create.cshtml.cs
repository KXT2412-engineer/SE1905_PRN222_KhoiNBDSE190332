using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Services.KhoiNBD;

namespace zCoach.RazorWebApp.KhoiNBD.Pages.MembershipFeesKhoiNbd;

[Authorize]
public class CreateModel : MembershipFeeFormPageModel
{
    private readonly IMembershipFeeKhoiNbdService _membershipFeeService;
    private readonly IFeePackageKhoiNbdService _feePackageService;

    public CreateModel(
        IMembershipFeeKhoiNbdService membershipFeeService,
        IFeePackageKhoiNbdService feePackageService)
    {
        _membershipFeeService = membershipFeeService;
        _feePackageService = feePackageService;
    }

    public async Task OnGetAsync()
    {
        MembershipFee = new MembershipFeeKhoiNbd
        {
            PaymentDate = DateTime.Today,
            PublishDate = DateTime.Today,
            IsActive = true
        };

        await LoadPackagesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        RemoveNavigationErrors();

        if (!ModelState.IsValid)
        {
            await LoadPackagesAsync();
            return Page();
        }

        await _membershipFeeService.CreateAsync(MembershipFee);
        TempData["Success"] = $"Đã tạo khoản phí cho \"{MembershipFee.MemberName}\".";
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

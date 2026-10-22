using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Services.KhoiNBD;

namespace zCoach.RazorWebApp.KhoiNBD.Pages.MembershipFeesKhoiNbd;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IMembershipFeeKhoiNbdService _membershipFeeService;

    public DeleteModel(IMembershipFeeKhoiNbdService membershipFeeService)
    {
        _membershipFeeService = membershipFeeService;
    }

    public MembershipFeeKhoiNbd MembershipFee { get; set; } = new();

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
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var deleted = await _membershipFeeService.DeleteAsync(id);
        TempData[deleted ? "Success" : "Error"] = deleted
            ? $"Đã xoá khoản phí #{id}."
            : $"Không tìm thấy khoản phí #{id}.";

        return RedirectToPage("./Index");
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using zSubscription.Entities.KhoiNBD.Models;

namespace zCoach.RazorWebApp.KhoiNBD.Pages.MembershipFeesKhoiNbd;

/// <summary>
/// Base cho Create/Edit để dùng chung partial _FormFields.
/// </summary>
public abstract class MembershipFeeFormPageModel : PageModel
{
    [BindProperty]
    public MembershipFeeKhoiNbd MembershipFee { get; set; } = new();
}

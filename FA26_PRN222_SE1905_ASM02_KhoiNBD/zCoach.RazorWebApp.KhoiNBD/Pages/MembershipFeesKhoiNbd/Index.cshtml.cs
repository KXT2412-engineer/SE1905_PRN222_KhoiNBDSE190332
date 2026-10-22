using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Services.KhoiNBD;

namespace zCoach.RazorWebApp.KhoiNBD.Pages.MembershipFeesKhoiNbd;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IMembershipFeeKhoiNbdService _membershipFeeService;

    public IndexModel(IMembershipFeeKhoiNbdService membershipFeeService)
    {
        _membershipFeeService = membershipFeeService;
    }

    public IList<MembershipFeeKhoiNbd> MembershipFees { get; set; } = new List<MembershipFeeKhoiNbd>();

    [BindProperty(SupportsGet = true)]
    public string? MemberName { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ActiveStatus { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? PackageName { get; set; }

    public async Task OnGetAsync()
    {
        MembershipFees = await _membershipFeeService.SearchAsync(MemberName, ActiveStatus, PackageName);
    }
}

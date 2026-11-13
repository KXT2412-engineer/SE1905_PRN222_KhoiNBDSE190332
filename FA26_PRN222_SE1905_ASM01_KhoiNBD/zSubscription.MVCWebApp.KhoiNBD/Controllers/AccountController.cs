using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zSubscription.MVCWebApp.KhoiNBD.Models;
using zSubscription.Services.KhoiNBD;

namespace zSubscription.MVCWebApp.KhoiNBD.Controllers;

[AllowAnonymous]
public sealed class AccountController : Controller
{
    private readonly ISystemUserAccountService _accounts;

    public AccountController(ISystemUserAccountService accounts) => _accounts = accounts;

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/MembershipFees");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel input)
    {
        if (!ModelState.IsValid)
            return View(input);

        var account = await _accounts.GetUserAccountAsync(input.UserName.Trim(), input.Password);
        if (account is null)
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(input);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.UserAccountId.ToString()),
            new(ClaimTypes.Name, account.FullName ?? account.UserName),
            new(ClaimTypes.Role, account.RoleId.ToString())
        };
        if (!string.IsNullOrWhiteSpace(account.Email))
            claims.Add(new Claim(ClaimTypes.Email, account.Email));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return LocalRedirect(Url.IsLocalUrl(input.ReturnUrl) ? input.ReturnUrl! : "/MembershipFees");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}

using System.ComponentModel.DataAnnotations;

namespace zSubscription.MVCWebApp.KhoiNBD.Models;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

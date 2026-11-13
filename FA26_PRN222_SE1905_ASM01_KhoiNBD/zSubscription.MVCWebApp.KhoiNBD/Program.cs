using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.MVCWebApp.KhoiNBD.Hubs;
using zSubscription.Repositories.KhoiNBD;
using zSubscription.Repositories.KhoiNBD.DBContext;
using zSubscription.Services.KhoiNBD;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "zSubscription.Mvc.Session";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization(options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

builder.Services.AddDbContext<PRN222Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<MembershipFeeKhoiNbdRepository>();
builder.Services.AddScoped<FeePackageKhoiNbdRepository>();
builder.Services.AddScoped<SystemUserAccountRepository>();
builder.Services.AddScoped<IMembershipFeeKhoiNbdService, MembershipFeeKhoiNbdService>();
builder.Services.AddScoped<IFeePackageKhoiNbdService, FeePackageKhoiNbdService>();
builder.Services.AddScoped<ISystemUserAccountService, SystemUserAccountService>();

var app = builder.Build();

// Gói mẫu để form khoản phí có lựa chọn và bộ lọc tên gói dùng được ngay.
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PRN222Context>();
    var existingNames = await db.FeePackageKhoiNbds
        .Select(package => package.PackageName)
        .ToListAsync();

    var samplePackages = new[]
    {
        new FeePackageKhoiNbd { PackageName = "Basic", Description = "Gói cơ bản 30 ngày", DurationInDays = 30, Price = 99000m, Status = 1, PublishDate = DateTime.Today },
        new FeePackageKhoiNbd { PackageName = "Premium", Description = "Gói premium 90 ngày", DurationInDays = 90, Price = 249000m, Status = 1, PublishDate = DateTime.Today },
        new FeePackageKhoiNbd { PackageName = "VIP", Description = "Gói VIP 365 ngày", DurationInDays = 365, Price = 799000m, Status = 1, PublishDate = DateTime.Today }
    };

    var missingPackages = samplePackages
        .Where(sample => !existingNames.Any(name => string.Equals(name, sample.PackageName, StringComparison.OrdinalIgnoreCase)))
        .ToList();

    if (missingPackages.Count > 0)
    {
        db.FeePackageKhoiNbds.AddRange(missingPackages);
        await db.SaveChangesAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<MembershipFeeHub>("/membershipFeeHub");

app.Run();

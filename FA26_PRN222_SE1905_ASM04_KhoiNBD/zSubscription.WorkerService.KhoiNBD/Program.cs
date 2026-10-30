using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging.EventLog;
using zSubscription.Repositories.KhoiNBD;
using zSubscription.Repositories.KhoiNBD.DBContext;
using zSubscription.Services.KhoiNBD;
using zSubscription.WorkerService.KhoiNBD;

// A Windows Service can start from System32. Resolve settings and output beside the executable.
var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = builder.Configuration["WindowsService:ServiceName"]
        ?? "KhoiNBD_PRN222_ASM04";
});

// Console/F5 mode must not require permission to register an Event Log source.
if (OperatingSystem.IsWindows() && !WindowsServiceHelpers.IsWindowsService())
{
    builder.Logging.AddFilter<EventLogLoggerProvider>(_ => false);
}

builder.Services.AddSingleton<MembershipFeeSnapshotWriter>();

builder.Services.AddDbContext<PRN222Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<MembershipFeeKhoiNbdRepository>();
builder.Services.AddScoped<FeePackageKhoiNbdRepository>();
builder.Services.AddScoped<SystemUserAccountRepository>();
builder.Services.AddScoped<IMembershipFeeKhoiNbdService, MembershipFeeKhoiNbdService>();
builder.Services.AddScoped<IFeePackageKhoiNbdService, FeePackageKhoiNbdService>();
builder.Services.AddScoped<IMembershipFeeMaintenanceService, MembershipFeeMaintenanceService>();

builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(WorkerOptions.SectionName));
builder.Services.AddHostedService<MembershipFeeWorker>();

var host = builder.Build();
host.Run();

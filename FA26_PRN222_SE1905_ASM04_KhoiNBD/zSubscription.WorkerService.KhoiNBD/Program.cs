using Microsoft.EntityFrameworkCore;
using zSubscription.Repositories.KhoiNBD;
using zSubscription.Repositories.KhoiNBD.DBContext;
using zSubscription.Services.KhoiNBD;
using zSubscription.WorkerService.KhoiNBD;

var builder = Host.CreateApplicationBuilder(args);

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

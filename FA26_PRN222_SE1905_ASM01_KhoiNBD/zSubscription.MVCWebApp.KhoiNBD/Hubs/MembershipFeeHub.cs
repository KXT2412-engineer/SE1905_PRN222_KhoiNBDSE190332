using Microsoft.AspNetCore.SignalR;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Services.KhoiNBD;

namespace zSubscription.MVCWebApp.KhoiNBD.Hubs;

/// <summary>
/// SignalR CRUD endpoint for Assignment 01. All database work goes through the
/// existing services, so the user's entities, mappings and connection remain unchanged.
/// </summary>
public sealed class MembershipFeeHub : Hub
{
    private readonly IMembershipFeeKhoiNbdService _fees;
    private readonly IFeePackageKhoiNbdService _packages;

    public MembershipFeeHub(
        IMembershipFeeKhoiNbdService fees,
        IFeePackageKhoiNbdService packages)
    {
        _fees = fees;
        _packages = packages;
    }

    public async Task<object?> GetById(int id)
    {
        var item = await _fees.GetByIdAsync(id);
        return item is null ? null : MapItem(item);
    }

    public async Task<IEnumerable<object>> GetPackages()
        => (await _packages.GetAllAsync()).Select(x => new
        {
            x.FeePackageKhoiNbdid,
            x.PackageName
        });

    public async Task<int> Create(MembershipFeeDto dto)
    {
        var item = MapDto(dto);
        item.MembershipFeeKhoiNbdid = 0;
        Validate(item);
        var created = await _fees.CreateAsync(item);
        await BroadcastAsync("created", created.MemberName);
        return created.MembershipFeeKhoiNbdid;
    }

    public async Task Update(MembershipFeeDto dto)
    {
        if (dto.MembershipFeeKhoiNbdid <= 0)
            throw new HubException("Invalid membership fee ID.");

        var item = MapDto(dto);
        Validate(item);
        if (!await _fees.UpdateAsync(item))
            throw new HubException("Membership fee was not found.");
        await BroadcastAsync("updated", item.MemberName);
    }

    public async Task Delete(int id)
    {
        var item = await _fees.GetByIdAsync(id)
            ?? throw new HubException("Membership fee was not found.");
        if (!await _fees.DeleteAsync(id))
            throw new HubException("Could not delete membership fee.");
        await BroadcastAsync("deleted", item.MemberName);
    }

    private static void Validate(MembershipFeeKhoiNbd item)
    {
        if (string.IsNullOrWhiteSpace(item.MemberName))
            throw new HubException("Member name is required.");
        if (item.Amount <= 0)
            throw new HubException("Amount must be greater than 0.");
    }

    private Task BroadcastAsync(string action, string? memberName)
        => Clients.All.SendAsync("MembershipFeeChanged", new
        {
            Action = action,
            MemberName = memberName,
            ChangedAt = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")
        });

    private static object MapItem(MembershipFeeKhoiNbd item) => new
    {
        item.MembershipFeeKhoiNbdid,
        item.MemberName,
        item.Amount,
        PaymentDate = item.PaymentDate.ToString("yyyy-MM-dd"),
        EndDate = item.EndDate?.ToString("yyyy-MM-dd"),
        item.Notes,
        item.Status,
        PublishDate = item.PublishDate?.ToString("yyyy-MM-dd"),
        item.FeePackageKhoiNbdid,
        item.IsActive
    };

    private static MembershipFeeKhoiNbd MapDto(MembershipFeeDto dto) => new()
    {
        MembershipFeeKhoiNbdid = dto.MembershipFeeKhoiNbdid,
        MemberName = dto.MemberName?.Trim() ?? string.Empty,
        Amount = dto.Amount,
        PaymentDate = dto.PaymentDate ?? DateTime.Today,
        EndDate = dto.EndDate,
        Notes = dto.Notes,
        Status = dto.Status,
        PublishDate = dto.PublishDate ?? DateTime.Today,
        FeePackageKhoiNbdid = dto.FeePackageKhoiNbdid,
        IsActive = dto.IsActive
    };
}

public sealed class MembershipFeeDto
{
    public int MembershipFeeKhoiNbdid { get; set; }
    public string? MemberName { get; set; }
    public decimal Amount { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
    public int? Status { get; set; }
    public DateTime? PublishDate { get; set; }
    public int? FeePackageKhoiNbdid { get; set; }
    public bool IsActive { get; set; } = true;
}

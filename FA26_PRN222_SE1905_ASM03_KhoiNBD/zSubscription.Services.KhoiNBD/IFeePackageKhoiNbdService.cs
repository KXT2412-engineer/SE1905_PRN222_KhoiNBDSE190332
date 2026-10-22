using zSubscription.Entities.KhoiNBD.Models;

namespace zSubscription.Services.KhoiNBD;

public interface IFeePackageKhoiNbdService
{
    Task<List<FeePackageKhoiNbd>> GetAllAsync();
}

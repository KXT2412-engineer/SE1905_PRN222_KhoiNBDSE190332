using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD;

namespace zSubscription.Services.KhoiNBD;

public class FeePackageKhoiNbdService : IFeePackageKhoiNbdService
{
    private readonly FeePackageKhoiNbdRepository _repository;

    public FeePackageKhoiNbdService(FeePackageKhoiNbdRepository repository)
    {
        _repository = repository;
    }

    public Task<List<FeePackageKhoiNbd>> GetAllAsync()
        => _repository.GetAllAsync();
}

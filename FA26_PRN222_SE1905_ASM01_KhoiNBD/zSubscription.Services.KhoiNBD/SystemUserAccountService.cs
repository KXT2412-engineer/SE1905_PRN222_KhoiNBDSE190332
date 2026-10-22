using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD;

namespace zSubscription.Services.KhoiNBD;

public class SystemUserAccountService : ISystemUserAccountService
{
    private readonly SystemUserAccountRepository _repository;

    public SystemUserAccountService(SystemUserAccountRepository repository)
    {
        _repository = repository;
    }

    public Task<SystemUserAccount?> GetUserAccountAsync(string userName, string password)
        => _repository.GetUserAccountAsync(userName, password);
}

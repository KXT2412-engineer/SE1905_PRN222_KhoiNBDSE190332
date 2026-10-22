using zSubscription.Entities.KhoiNBD.Models;

namespace zSubscription.Services.KhoiNBD;

public interface ISystemUserAccountService
{
    Task<SystemUserAccount?> GetUserAccountAsync(string userName, string password);
}

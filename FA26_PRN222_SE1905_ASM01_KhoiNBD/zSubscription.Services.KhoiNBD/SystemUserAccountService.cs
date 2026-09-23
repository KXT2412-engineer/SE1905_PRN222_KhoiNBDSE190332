using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD;

namespace zSubscription.Services.KhoiNBD
{
    public class SystemUserAccountService : ISystemUserAccountService
    {
        private readonly SystemUserAccountRepository _repository;
        public SystemUserAccountService() => _repository = new SystemUserAccountRepository();
        public async Task<SystemUserAccount> GetUserAccount(string userName, string password)
        {
            try 
            { 
                return await _repository.GetByUserNameAsync(userName, password);
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in GetUserAccount: ", ex);
            }
        }
    }
}

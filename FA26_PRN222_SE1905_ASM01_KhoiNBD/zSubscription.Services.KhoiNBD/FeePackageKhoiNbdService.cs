using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD;

namespace zSubscription.Services.KhoiNBD
{
    public class FeePackageKhoiNbdService : iFeePackageKhoiNbdService
    {
        private readonly FeePackageKhoiNbdRepository _repository;
        public FeePackageKhoiNbdService() => _repository = new FeePackageKhoiNbdRepository();
        public async Task<List<FeePackageKhoiNbd>> GetAllAsync()
        {
            try 
            {
                return await _repository.GetAllAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Error in GetAllAsync: " + ex);
            }
            //throw new NotImplementedException();
        }
    }
}

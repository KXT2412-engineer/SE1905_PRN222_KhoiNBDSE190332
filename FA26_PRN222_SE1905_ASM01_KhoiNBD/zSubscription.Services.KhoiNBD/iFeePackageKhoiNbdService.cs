using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zSubscription.Services.KhoiNBD
{
    public interface iFeePackageKhoiNbdService
    {
        Task<List<Entities.KhoiNBD.Models.FeePackageKhoiNbd>> GetAllAsync();
    }
}
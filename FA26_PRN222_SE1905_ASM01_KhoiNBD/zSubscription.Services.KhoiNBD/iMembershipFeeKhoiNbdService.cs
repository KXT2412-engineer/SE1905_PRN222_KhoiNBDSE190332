using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zSubscription.Entities.KhoiNBD.Models;

namespace zSubscription.Services.KhoiNBD
{
    public interface iMembershipFeeKhoiNbdService
    {
        Task<List<MembershipFeeKhoiNbd>> GetAllAsync();
        Task<MembershipFeeKhoiNbd> GetByIdAsync(int id);
        Task<List<MembershipFeeKhoiNbd>> SearchAsync(string transCode, decimal amount, string payMethod);
        Task<int> CreateAsync(MembershipFeeKhoiNbd membershipFee);
        Task<int> UpdateAsync(MembershipFeeKhoiNbd membershipFee);
        Task<bool> DeleteAsync(Guid id);
    }
}

using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD;

namespace zSubscription.Services.KhoiNBD
{
    public class MembershipFeeKhoiNbdService : iMembershipFeeKhoiNbdService
    {
        private readonly MembershipFeeKhoiNbdRepository _repository;
        public MembershipFeeKhoiNbdService() => _repository = new MembershipFeeKhoiNbdRepository();
        public async Task<int> CreateAsync(MembershipFeeKhoiNbd membershipFee)
        {
            try 
            {
                return await Task.Run(() => _repository.CreateAsync(membershipFee));
            } 
            catch (Exception ex) { throw new ApplicationException("Error creating Membership fee " + ex.Message, ex); }
            //throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try 
            {
                var item = await _repository.GetByIdAsync(id);
                if (item == null) return false;
                return await _repository.RemoveAsync(item);
            } 
            catch (Exception ex) { throw new ApplicationException("Error deleting Membership fee " + ex.Message, ex); }
        }

        public async Task<List<MembershipFeeKhoiNbd>> GetAllAsync()
        {
            try
            {
                return await Task.Run(() => _repository.GetAllAsync());
            }
            catch (Exception ex) { throw new ApplicationException("Error retrieving Membership fee list " + ex.Message, ex); }
            //throw new NotImplementedException();
        }

        public async Task<MembershipFeeKhoiNbd> GetByIdAsync(int id)
        {
            try
            {
                return await Task.Run(() => _repository.GetByIdAsync(id));
            }
            catch (Exception ex) { throw new ApplicationException("Error retrieving Membership fee by ID " + ex.Message, ex); }
            //throw new NotImplementedException();
        }

        public async Task<List<MembershipFeeKhoiNbd>> SearchAsync(string transCode, decimal amount, string payMethod)
        {
            try 
            {
                return await Task.Run(() => _repository.SearchAsync(amount, transCode, payMethod));
            } 
            catch (Exception ex) { throw new ApplicationException("Error searching Membership fee " + ex.Message, ex); }
            //throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(MembershipFeeKhoiNbd membershipFee)
        {
            try 
            {
                return Task.Run(() => _repository.UpdateAsync(membershipFee));
            } 
            catch (Exception ex) { throw new ApplicationException("Error updating Membership fee " + ex.Message, ex); }
            //throw new NotImplementedException();
        }
    }
}

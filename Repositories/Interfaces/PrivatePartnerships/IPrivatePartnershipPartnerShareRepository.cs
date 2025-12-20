using ERP.Models.PrivatePartnerships;

namespace ERP.Repositories.Interfaces.PrivatePartnerships
{
    public interface IPrivatePartnershipPartnerShareRepository : IBaseRepository<PrivatePartnershipPartnerShare>
    {
        Task AddRangeAsync(IEnumerable<PrivatePartnershipPartnerShare> shares, int userId);
    }
}

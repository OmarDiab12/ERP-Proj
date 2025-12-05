using ERP.Models;
using ERP.Models.PrivatePartnerships;
using ERP.Repositories.Interfaces.PrivatePartnerships;

namespace ERP.Repositories.PrivatePartnerships
{
    public class PrivatePartnershipPartnerShareRepository : BaseRepository<PrivatePartnershipPartnerShare>, IPrivatePartnershipPartnerShareRepository
    {
        public PrivatePartnershipPartnerShareRepository(ERPDBContext context) : base(context)
        {
        }

        public async Task AddRangeAsync(IEnumerable<PrivatePartnershipPartnerShare> shares, int userId)
        {
            var now = DateTime.UtcNow;
            foreach (var share in shares)
            {
                share.CreatedAt = now;
                share.UpdatedAt = now;
                share.CreatedBy = userId;
                share.UpdatedBy = userId;
            }

            await _context.PrivatePartnershipPartnerShares.AddRangeAsync(shares);
            await _context.SaveChangesAsync();
        }
    }
}

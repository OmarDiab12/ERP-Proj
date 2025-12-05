using ERP.Models;
using ERP.Models.PrivatePartnerships;
using ERP.Repositories.Interfaces.PrivatePartnerships;

namespace ERP.Repositories.PrivatePartnerships
{
    public class PrivatePartnershipTransactionRepository : BaseRepository<PrivatePartnershipTransaction>, IPrivatePartnershipTransactionRepository
    {
        public PrivatePartnershipTransactionRepository(ERPDBContext context) : base(context)
        {
        }
    }
}

using ERP.Models;
using ERP.Models.QoutationManagement;

namespace ERP.Repositories
{
    public class QuotationRepository : BaseRepository<Quotation>, IQuotationRepository
    {
        public QuotationRepository(ERPDBContext context) : base(context) { }
        
    }
}

using ERP.Models;
using ERP.Models.QoutationManagement;
using ERP.Repositories.Interfaces.QuotationManagement;

namespace ERP.Repositories.QuotationManagement
{
    public class QuotationRepository : BaseRepository<Quotation>, IQuotationRepository
    {
        public QuotationRepository(ERPDBContext context) : base(context) { }
        
    }
}

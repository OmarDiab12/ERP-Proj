using ERP.Models;
using ERP.Models.QoutationManagement;
using ERP.Repositories.Interfaces.QuotationManagement;

namespace ERP.Repositories.QuotationManagement
{
    public class QuotationRepository : BaseRepository<Quotation>, IQuotationRepository
    {
        public QuotationRepository(ERPDBContext context) : base(context) { }

        public bool ChangeStatus(int quotationId ,QuotationStatus status)
        {
            var q = _context.Quotations.Where(c=>c.Id == quotationId && !c.IsDeleted).FirstOrDefault();
            q.Status = status;
            _context.SaveChangesAsync();
            return true;
        }
        
    }
}

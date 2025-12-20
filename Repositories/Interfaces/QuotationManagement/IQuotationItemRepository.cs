using ERP.Models.QoutationManagement;

namespace ERP.Repositories.Interfaces.QuotationManagement
{
    public interface IQuotationItemRepository : IBaseRepository<QuotationItem>
    {
        Task<List<QuotationItem>> GetByQuotationIdAsync(int quotationId);
        Task DeleteByQuotationIdAsync(int quotationId);
    }
}

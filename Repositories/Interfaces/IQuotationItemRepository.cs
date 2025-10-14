using ERP.Models.QoutationManagement;

namespace ERP.Repositories.Interfaces
{
    public interface IQuotationItemRepository : IBaseRepository<QuotationItem>
    {
        Task<List<QuotationItem>> GetByQuotationIdAsync(int quotationId);
        Task DeleteByQuotationIdAsync(int quotationId);
    }
}

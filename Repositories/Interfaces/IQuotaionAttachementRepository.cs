using ERP.Models.QoutationManagement;

namespace ERP.Repositories.Interfaces
{
    public interface IQuotaionAttachementRepository : IBaseRepository<QuotationAttachement>
    {
        Task DeleteByQuotationIdAsync(int quotationId);
    }
}

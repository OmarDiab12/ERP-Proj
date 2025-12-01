using ERP.Models.InvoicesManagement;

namespace ERP.Repositories.Interfaces.Invoices
{
    public interface IInvoiceRepository : IBaseRepository<Invoice>
    {
        Task<Invoice?> GetWithDetailsAsync(int id);
        Task<List<Invoice>> GetAllWithDetailsAsync();
        Task<bool> ReplaceItemsAsync(int invoiceId, List<InvoiceItem> items, int updatedBy);
        Task<bool> AddAttachmentsAsync(int invoiceId, List<InvoiceAttachment> attachments, int updatedBy);
    }
}

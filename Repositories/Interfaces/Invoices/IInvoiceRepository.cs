using ERP.Models.InvoicesManagement;

namespace ERP.Repositories.Interfaces.Invoices
{
    public interface IInvoiceRepository : IBaseRepository<Invoice>
    {
        Task<Invoice?> GetWithDetailsAsync(int id);
        Task<List<Invoice>> GetAllWithDetailsAsync();
        Task<bool> ReplaceItemsAsync(int invoiceId, List<InvoiceItem> items, int updatedBy);
        Task<bool> AddAttachmentsAsync(int invoiceId, List<InvoiceAttachment> attachments, int updatedBy);
        Task<bool> ReplaceSchedulesAsync(int invoiceId, List<InvoicePaymentSchedule> schedules, int updatedBy);
        Task<List<Invoice>> GetFilteredAsync(Helpers.Enums.InvoiceType? type, Helpers.Enums.InvoicePaymentType? paymentType, Helpers.Enums.InvoiceStatus? status, int? supplierId, int? clientId, int? projectId, DateTime? from, DateTime? to);
    }
}

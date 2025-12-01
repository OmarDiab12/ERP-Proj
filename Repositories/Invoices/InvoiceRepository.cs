using ERP.Models.InvoicesManagement;
using ERP.Repositories.Interfaces.Invoices;
using ERP.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.Invoices
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(ERPDBContext context) : base(context)
        {
        }

        public async Task<bool> AddAttachmentsAsync(int invoiceId, List<InvoiceAttachment> attachments, int updatedBy)
        {
            var invoice = await _dbSet.Include(i => i.Attachments).FirstOrDefaultAsync(i => i.Id == invoiceId && !i.IsDeleted);
            if (invoice == null)
                return false;

            foreach (var attachment in attachments)
            {
                attachment.InvoiceId = invoiceId;
                attachment.CreatedAt = DateTime.UtcNow;
                attachment.UpdatedAt = DateTime.UtcNow;
                attachment.CreatedBy = updatedBy;
                attachment.UpdatedBy = updatedBy;
            }

            await _context.Set<InvoiceAttachment>().AddRangeAsync(attachments);
            invoice.UpdatedBy = updatedBy;
            invoice.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Invoice>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(i => i.Items)
                .Include(i => i.Attachments)
                .Where(i => !i.IsDeleted)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<Invoice?> GetWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(i => i.Items)
                .Include(i => i.Attachments)
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        }

        public async Task<bool> ReplaceItemsAsync(int invoiceId, List<InvoiceItem> items, int updatedBy)
        {
            var invoice = await _dbSet.Include(i => i.Items).FirstOrDefaultAsync(i => i.Id == invoiceId && !i.IsDeleted);
            if (invoice == null)
                return false;

            var existingItems = await _context.Set<InvoiceItem>().Where(i => i.InvoiceId == invoiceId).ToListAsync();
            _context.Set<InvoiceItem>().RemoveRange(existingItems);

            foreach (var item in items)
            {
                item.InvoiceId = invoiceId;
                item.CreatedAt = DateTime.UtcNow;
                item.UpdatedAt = DateTime.UtcNow;
                item.CreatedBy = updatedBy;
                item.UpdatedBy = updatedBy;
            }

            await _context.Set<InvoiceItem>().AddRangeAsync(items);
            invoice.UpdatedBy = updatedBy;
            invoice.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

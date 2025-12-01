using ERP.Helpers;
using ERP.Models.InvoicesManagement;
using ERP.Repositories.Interfaces.Invoices;
using ERP.Repositories;
using Microsoft.EntityFrameworkCore;
using ERP.Models;

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
                .Include(i => i.PaymentSchedules)
                .Where(i => !i.IsDeleted)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<Invoice?> GetWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(i => i.Items)
                .Include(i => i.Attachments)
                .Include(i => i.PaymentSchedules)
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

        public async Task<bool> ReplaceSchedulesAsync(int invoiceId, List<InvoicePaymentSchedule> schedules, int updatedBy)
        {
            var invoice = await _dbSet.Include(i => i.PaymentSchedules).FirstOrDefaultAsync(i => i.Id == invoiceId && !i.IsDeleted);
            if (invoice == null)
                return false;

            var existingSchedules = await _context.Set<InvoicePaymentSchedule>().Where(s => s.InvoiceId == invoiceId).ToListAsync();
            _context.Set<InvoicePaymentSchedule>().RemoveRange(existingSchedules);

            foreach (var schedule in schedules)
            {
                schedule.InvoiceId = invoiceId;
                schedule.CreatedAt = DateTime.UtcNow;
                schedule.UpdatedAt = DateTime.UtcNow;
                schedule.CreatedBy = updatedBy;
                schedule.UpdatedBy = updatedBy;
            }

            await _context.Set<InvoicePaymentSchedule>().AddRangeAsync(schedules);
            invoice.UpdatedAt = DateTime.UtcNow;
            invoice.UpdatedBy = updatedBy;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Invoice>> GetFilteredAsync(Enums.InvoiceType? type, Enums.InvoicePaymentType? paymentType, Enums.InvoiceStatus? status, int? supplierId, int? clientId, int? projectId, DateTime? from, DateTime? to)
        {
            var query = _dbSet
                .Include(i => i.Items)
                .Include(i => i.Attachments)
                .Include(i => i.PaymentSchedules)
                .Where(i => !i.IsDeleted)
                .AsQueryable();

            if (type.HasValue)
                query = query.Where(i => i.Type == type.Value);
            if (paymentType.HasValue)
                query = query.Where(i => i.PaymentType == paymentType.Value);
            if (status.HasValue)
                query = query.Where(i => i.Status == status.Value);
            if (supplierId.HasValue)
                query = query.Where(i => i.SupplierId == supplierId);
            if (clientId.HasValue)
                query = query.Where(i => i.ClientId == clientId);
            if (projectId.HasValue)
                query = query.Where(i => i.ProjectId == projectId);
            if (from.HasValue)
                query = query.Where(i => i.InvoiceDate >= from.Value);
            if (to.HasValue)
                query = query.Where(i => i.InvoiceDate <= to.Value);

            return await query
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }
    }
}

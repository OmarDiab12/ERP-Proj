using ERP.Models;
using ERP.Models.QoutationManagement;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.QuotationManagement
{
    public class QuotaionAttachementRepository : BaseRepository<QuotationAttachement>, IQuotaionAttachementRepository
    {
        public QuotaionAttachementRepository(ERPDBContext context) : base(context)
        {
        }

        public async Task DeleteByQuotationIdAsync(int quotationId)
        {
            // حذف جميع العناصر المرتبطة بـ quotationId مع SaveChanges داخل معاملة
            using (var tx = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var items = await _context.Set<QuotationAttachement>()
                                              .Where(i => i.QuotationId == quotationId)
                                              .ToListAsync();

                    if (items != null && items.Count > 0)
                    {
                        _context.Set<QuotationAttachement>().RemoveRange(items);
                        await _context.SaveChangesAsync();
                    }

                    await tx.CommitAsync();
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
            }
        }
    }
}

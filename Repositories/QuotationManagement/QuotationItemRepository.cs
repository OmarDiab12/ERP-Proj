using ERP.Models;
using ERP.Models.QoutationManagement;
using ERP.Repositories.Interfaces.QuotationManagement;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.QuotationManagement
{
    public class QuotationItemRepository : BaseRepository<QuotationItem>, IQuotationItemRepository
    {
        public QuotationItemRepository(ERPDBContext context) : base(context) { }

        public async Task<List<QuotationItem>> GetByQuotationIdAsync(int quotationId)
        {
            // إرجاع العناصر المرتبطة بالـ quotationId مرتبة حسب Id أو أي ترتيب تفضله
            return await _context.Set<QuotationItem>()
                                 .Where(i => i.QuotationId == quotationId)
                                 .OrderBy(i => i.Id)
                                 .ToListAsync();
        }

        public async Task DeleteByQuotationIdAsync(int quotationId)
        {
            // حذف جميع العناصر المرتبطة بـ quotationId مع SaveChanges داخل معاملة
            using (var tx = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var items = await _context.Set<QuotationItem>()
                                              .Where(i => i.QuotationId == quotationId)
                                              .ToListAsync();

                    if (items != null && items.Count > 0)
                    {
                        _context.Set<QuotationItem>().RemoveRange(items);
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

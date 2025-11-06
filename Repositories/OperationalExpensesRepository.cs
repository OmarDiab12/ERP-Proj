using ERP.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories
{
    public class OperationalExpensesRepository : BaseRepository<OperationalExpense>, IOperationalExpensesRepository
    {
        public OperationalExpensesRepository(ERPDBContext context) : base(context) { }
        public async Task<List<OperationalExpense>> GetByDateRangeAsync(DateTime from, DateTime to, string? category = null)
        {
            var q = _context.Set<OperationalExpense>()
                            .Where(e => e.ExpenseDate >= from && e.ExpenseDate <= to && !e.IsDeleted);

            if (!string.IsNullOrWhiteSpace(category))
                q = q.Where(e => e.Category == category);

            return await q.OrderByDescending(e => e.ExpenseDate).ToListAsync();
        }

        public async Task<List<OperationalExpense>> GetPagedAsync(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            return await _context.Set<OperationalExpense>()
                                  .Where(e=>!e.IsDeleted)
                                 .OrderByDescending(e => e.ExpenseDate)
                                 .Skip(skip).Take(pageSize)
                                 .ToListAsync();
        }

        public async Task<decimal> GetTotalByDateRangeAsync(DateTime from, DateTime to, string? category = null)
        {
            var q = _context.Set<OperationalExpense>()
                            .Where(e => e.ExpenseDate >= from && e.ExpenseDate <= to && !e.IsDeleted);

            if (!string.IsNullOrWhiteSpace(category))
                q = q.Where(e => e.Category == category);

            return await q.SumAsync(e => (decimal?)e.Amount) ?? 0m;
        }
    }
}

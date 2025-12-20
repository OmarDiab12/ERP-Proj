using ERP.Models;

namespace ERP.Repositories.Interfaces
{
    public interface IOperationalExpensesRepository : IBaseRepository<OperationalExpense>
    {
        Task<List<OperationalExpense>> GetByDateRangeAsync(DateTime from, DateTime to, string? category = null);
        Task<List<OperationalExpense>> GetPagedAsync(int page, int pageSize);
        Task<decimal> GetTotalByDateRangeAsync(DateTime from, DateTime to, string? category = null);
    }
}

using ERP.Models;
using ERP.Models.Employees;
using ERP.Repositories.Interfaces.Persons;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.Persons
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ERPDBContext context) : base(context) { }

        public async Task<List<EmployeeTransaction>> GetEmployeeTransactionsAsync(int employeeId)
        {
            return await _context.Set<EmployeeTransaction>()
                .Where(t => t.EmployeeId == employeeId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<List<EmployeeTransaction>> GetEmployeeTransactionsPagedAsync(int employeeId, int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            return await _context.Set<EmployeeTransaction>()
                .Where(t => t.EmployeeId == employeeId)
                .OrderByDescending(t => t.TransactionDate)
                .Skip(skip).Take(pageSize)
                .ToListAsync();
        }

        public Task<EmployeeTransaction?> GetTransactionByIdAsync(int transactionId)
        {
            return _context.Set<EmployeeTransaction>()
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task<bool> AddTransactionAsync(EmployeeTransaction tx)
        {
            await _context.Set<EmployeeTransaction>().AddAsync(tx);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTransactionAsync(EmployeeTransaction tx)
        {
            _context.Set<EmployeeTransaction>().Update(tx);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTransactionAsync(int transactionId)
        {
            var tx = await _context.Set<EmployeeTransaction>().FindAsync(transactionId);
            if (tx == null) return false;
            _context.Set<EmployeeTransaction>().Remove(tx);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetEmployeeBalanceAsync(int employeeId)
        {
            var sum = await _context.Set<EmployeeTransaction>()
                .Where(t => t.EmployeeId == employeeId)
                .SumAsync(t => (decimal?)t.Amount) ?? 0m;

            return sum;
        }

    }
}

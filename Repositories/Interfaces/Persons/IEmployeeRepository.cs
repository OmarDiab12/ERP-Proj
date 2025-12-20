using ERP.Models.Employees;

namespace ERP.Repositories.Interfaces.Persons
{
    public interface IEmployeeRepository :IBaseRepository<Employee>
    {
        Task<List<EmployeeTransaction>> GetEmployeeTransactionsAsync(int employeeId);
        Task<List<EmployeeTransaction>> GetEmployeeTransactionsPagedAsync(int employeeId, int page, int pageSize);
        Task<EmployeeTransaction?> GetTransactionByIdAsync(int transactionId);
        Task<bool> AddTransactionAsync(EmployeeTransaction tx);
        Task<bool> UpdateTransactionAsync(EmployeeTransaction tx);
        Task<bool> DeleteTransactionAsync(int transactionId);

        Task<decimal> GetEmployeeBalanceAsync(int employeeId);
    }
}

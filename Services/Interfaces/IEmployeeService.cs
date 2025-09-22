using ERP.DTOs.Employee;

namespace ERP.Services.Interfaces
{
    public interface IEmployeeService
    {// Employees
        Task<ResponseDTO> CreateEmployeeAsync(CreateEmployeeDTO dto, int userId);
        Task<ResponseDTO> EditEmployeeAsync(EditEmployeeDTO dto, int userId);
        Task<ResponseDTO> DeleteEmployeeAsync(int employeeId, int userId);
        Task<ResponseDTO> GetEmployeeAsync(int employeeId);
        Task<ResponseDTO> GetAllEmployeesAsync();

        // Transactions
        Task<ResponseDTO> AddTransactionAsync(CreateEmployeeTransactionDTO dto, int userId);
        Task<ResponseDTO> EditTransactionAsync(EditEmployeeTransactionDTO dto, int userId);
        Task<ResponseDTO> DeleteTransactionAsync(int transactionId, int userId);
        Task<ResponseDTO> GetEmployeeTransactionsAsync(int employeeId, int page = 1, int pageSize = 50);
    }
}

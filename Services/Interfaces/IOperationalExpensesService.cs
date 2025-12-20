using ERP.DTOs.OperationalExpenses;

namespace ERP.Services.Interfaces
{
    public interface IOperationalExpensesService
    {
        Task<ResponseDTO> CreateAsync(CreateOperationalExpenseDTO dto, int userId);
        Task<ResponseDTO> EditAsync(EditOperationalExpenseDTO dto, int userId);
        Task<ResponseDTO> DeleteAsync(int id, int userId);
        Task<ResponseDTO> GetAsync(int id);
        Task<ResponseDTO> GetAllAsync(int page = 1, int pageSize = 50);
        Task<ResponseDTO> GetByDateRangeAsync(ExpenseRangeRequestDTO req);
    }
}

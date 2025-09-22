using ERP.DTOs.PersonalLoans;

namespace ERP.Services.Interfaces
{
    public interface IPersonalLoanService
    {
        Task<ResponseDTO> CreateAsync(CreatePersonalLoanDTO dto, int userId);
        Task<ResponseDTO> EditAsync(EditPersonalLoanDTO dto, int userId);
        Task<ResponseDTO> DeleteAsync(int id, int userId);
        Task<ResponseDTO> GetAsync(int id);
        Task<ResponseDTO> GetAllAsync(int page = 1, int pageSize = 50);
        Task<ResponseDTO> GetOverdueAsync(string asOfDate);
    }
}

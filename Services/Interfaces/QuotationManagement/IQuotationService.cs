using ERP.DTOs.Qiotation;

namespace ERP.Services.Interfaces.QuotationManagement
{
    public interface IQuotationService
    {
        Task<ResponseDTO> CreateAsync(CreateQuotationDTO dto, int userId);
        Task<ResponseDTO> EditAsync(EditQuotationDTO dto, int userId);
        Task<ResponseDTO> DeleteAsync(int id, int userId);
        Task<ResponseDTO> GetAsync(int id);
        Task<ResponseDTO> GetAllAsync();
    }
}

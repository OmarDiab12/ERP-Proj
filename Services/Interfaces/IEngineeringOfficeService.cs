using ERP.DTOs.EngineeringOffice;

namespace ERP.Services.Interfaces
{
    public interface IEngineeringOfficeService
    {
        Task<ResponseDTO> CreateAsync(CreateEngineeringProjectDTO dto, int userId);
        Task<ResponseDTO> UpdateAsync(UpdateEngineeringProjectDTO dto, int userId);
        Task<ResponseDTO> DeleteAsync(int id, int userId);
        Task<ResponseDTO> GetAsync(int id);
        Task<ResponseDTO> GetAllAsync();
    }
}

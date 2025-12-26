using ERP.DTOs.Tasks;

namespace ERP.Services.Interfaces.Tasks
{
    public interface ITaskService
    {
        Task<ResponseDTO> CreateAsync(TaskCreateDto dto, int userId);
        Task<ResponseDTO> UpdateAsync(TaskUpdateDto dto, int userId);
        Task<ResponseDTO> DeleteAsync(int id, int userId);
        Task<ResponseDTO> GetAsync(int id);
        Task<ResponseDTO> GetAllAsync(TaskFilterDto filter);
    }
}

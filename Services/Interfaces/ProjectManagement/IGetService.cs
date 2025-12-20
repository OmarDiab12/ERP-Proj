using ERP.DTOs.Projects;

namespace ERP.Services.Interfaces.ProjectManagement
{
    public interface IGetService
    {
        Task<ResponseDTO> GetAllAsync(ProjectFilterDto filter);
        Task<ResponseDTO> GetByIdAsync(int id);
    }
}

using ERP.DTOs.Projects;

namespace ERP.Services.Interfaces.ProjectManagement
{
    public interface IUpdateService
    {
        Task<ResponseDTO> UpdateProjectAsync(ProjectUpdateFullDTO dto, int userId);
    }
}

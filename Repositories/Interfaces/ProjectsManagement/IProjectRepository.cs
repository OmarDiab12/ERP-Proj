using ERP.Models.Projects;

namespace ERP.Repositories.Interfaces.ProjectsManagement
{
    public interface IProjectRepository : IBaseRepository<Project>
    {
        Task<Project?> GetFullProjectByIdAsync(int id);
        Task<IEnumerable<Project>> GetActiveProjectsAsync();
        Task<decimal> GetTotalProfitAsync(int projectId);
    }
}

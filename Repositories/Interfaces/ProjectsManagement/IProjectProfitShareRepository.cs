using ERP.Models.ProjectsManagement;

namespace ERP.Repositories.Interfaces.ProjectsManagement
{
    public interface IProjectProfitShareRepository : IBaseRepository<ProjectProfitShare>
    {
        Task<IEnumerable<ProjectProfitShare>> GetByProjectIdAsync(int projectId);
    }
}

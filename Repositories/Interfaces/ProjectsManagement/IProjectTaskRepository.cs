using ERP.Models.ProjectsManagement;

namespace ERP.Repositories.Interfaces.ProjectsManagement
{
    public interface IProjectTaskRepository : IBaseRepository<ProjectTask>
    {
        Task<IEnumerable<ProjectTask>> GetByProjectIdAsync(int projectId);
        Task<IEnumerable<ProjectTask>> GetByStatusAsync(Helpers.Enums.TaskStatus status);
    }
}

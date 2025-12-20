using ERP.Models.ProjectsManagement;

namespace ERP.Repositories.Interfaces.ProjectsManagement
{
    public interface IProjectAttachmentRepository : IBaseRepository<ProjectAttachment>
    {
        Task<IEnumerable<ProjectAttachment>> GetByProjectIdAsync(int projectId);
    }
}

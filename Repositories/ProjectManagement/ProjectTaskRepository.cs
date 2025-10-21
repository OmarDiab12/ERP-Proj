using ERP.Models;
using ERP.Models.ProjectsManagement;
using ERP.Repositories.Interfaces.ProjectsManagement;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.ProjectManagement
{
    public class ProjectTaskRepository : BaseRepository<ProjectTask> ,IProjectTaskRepository
    {
        public ProjectTaskRepository(ERPDBContext context) : base(context) { }

        public async Task<IEnumerable<ProjectTask>> GetByProjectIdAsync(int projectId)
        {
            return await _dbSet
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectTask>> GetByStatusAsync(ERP.Helpers.Enums.TaskStatus status)
        {
            return await _dbSet
                .Where(t => t.Status == status && !t.IsDeleted)
                .ToListAsync();
        }
    }
}
 
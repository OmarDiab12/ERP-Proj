using ERP.Models;
using ERP.Models.ProjectsManagement;
using ERP.Repositories.Interfaces.ProjectsManagement;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.ProjectManagement
{
    public class ProjectAttachmentRepository : BaseRepository<ProjectAttachment> ,IProjectAttachmentRepository
    {
        public ProjectAttachmentRepository(ERPDBContext context) : base(context) { }

        public async Task<IEnumerable<ProjectAttachment>> GetByProjectIdAsync(int projectId)
        {
            return await _dbSet
                .Where(a => a.ProjectId == projectId && !a.IsDeleted)
                .ToListAsync();
        }
    }
}

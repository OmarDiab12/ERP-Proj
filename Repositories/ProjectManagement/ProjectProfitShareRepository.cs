using ERP.Models;
using ERP.Models.ProjectsManagement;
using ERP.Repositories.Interfaces.ProjectsManagement;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.ProjectManagement
{
    public class ProjectProfitShareRepository : BaseRepository<ProjectProfitShare> ,IProjectProfitShareRepository
    {
        public ProjectProfitShareRepository(ERPDBContext context) : base(context) { }

        public async Task<IEnumerable<ProjectProfitShare>> GetByProjectIdAsync(int projectId)
        {
            return await _dbSet
                .Where(p => p.ProjectId == projectId && !p.IsDeleted)
                .ToListAsync();
        }
    }
}

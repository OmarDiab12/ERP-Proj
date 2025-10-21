using ERP.Models;
using ERP.Models.Projects;
using ERP.Repositories.Interfaces.ProjectsManagement;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.ProjectManagement
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ERPDBContext context) : base(context) { }

        public async Task<Project?> GetFullProjectByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            return await _dbSet.Where(p => p.Status == ProjectStatus.InProgress && !p.IsDeleted).ToListAsync();
        }

        public async Task<decimal> GetTotalProfitAsync(int projectId)
        {
            var project = await _dbSet.FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return 0;

            decimal totalPayments = project.ProjectPayments.Sum(x => x.Amount);
            decimal totalExpenses = project.ProjectExpenses.Sum(x => x.Amount);
            decimal totalContractorPayments = project.ContractOfContracts.Sum(x => x.ContractAmount);

            return totalPayments - (totalExpenses + totalContractorPayments);
        }
    }
}

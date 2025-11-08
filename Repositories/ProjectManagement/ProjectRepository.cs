using ERP.Models;
using ERP.Models.Projects;
using ERP.Repositories.Interfaces.ProjectsManagement;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
            var project = await _dbSet.FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);

            if (project == null) return 0;

            decimal totalPayments = project.ProjectPayments.Sum(x => x.Amount);
            decimal totalExpenses = project.ProjectExpenses.Sum(x => x.Amount);
            decimal totalContractorPayments = project.ContractOfContracts.Sum(x => x.ContractAmount);

            return totalPayments - (totalExpenses + totalContractorPayments);
        }

        public async Task SoftDeleteTaskAsync(int taskId, int userId)
        {
            var entity = await _context.ProjectTasks.FirstOrDefaultAsync(t => t.Id == taskId);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdatedBy = userId;
                entity.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SoftDeleteProfitShareAsync(int profitShareId, int userId)
        {
            var entity = await _context.ProjectProfitShares.FirstOrDefaultAsync(t => t.Id == profitShareId);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdatedBy = userId;
                entity.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SoftDeletePaymentAsync(int paymentId, int userId)
        {
            var entity = await _context.ProjectPayments.FirstOrDefaultAsync(t => t.Id == paymentId);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdatedBy = userId;
                entity.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SoftDeleteExpenseAsync(int expenseId, int userId)
        {
            var entity = await _context.ProjectExpenses.FirstOrDefaultAsync(t => t.Id == expenseId);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdatedBy = userId;
                entity.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}

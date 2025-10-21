using ERP.Models;
using ERP.Models.ProjectsManagement;
using ERP.Repositories.Interfaces.ProjectsManagement;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.ProjectManagement
{
    public class ProjectPaymentRepository :BaseRepository<ProjectPayment> , IProjectPaymentRepository
    {
        public ProjectPaymentRepository(ERPDBContext context) : base(context) { }

        public async Task<IEnumerable<ProjectPayment>> GetByProjectIdAsync(int projectId)
        {
            return await _dbSet
                .Where(p => p.ProjectId == projectId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalIncomeAsync(int projectId)
        {
            return await _dbSet
                .Where(p => p.ProjectId == projectId && p.Type == PaymentType.Income && !p.IsDeleted)
                .SumAsync(p => p.Amount);
        }
    }
}

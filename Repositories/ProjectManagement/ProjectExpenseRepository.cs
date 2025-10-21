using ERP.Models;
using ERP.Models.ProjectsManagement;
using ERP.Repositories.Interfaces.ProjectsManagement;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.ProjectManagement
{
    public class ProjectExpenseRepository : BaseRepository<ProjectExpense> ,IProjectExpenseRepository
    {
        public ProjectExpenseRepository(ERPDBContext context) : base(context) { }

        public async Task<IEnumerable<ProjectExpense>> GetByProjectIdAsync(int projectId)
        {
            return await _dbSet.Where(e => e.ProjectId == projectId && !e.IsDeleted).ToListAsync();
        }

        public async Task<decimal> GetTotalExpensesAsync(int projectId)
        {
            return await _dbSet
                .Where(e => e.ProjectId == projectId && !e.IsDeleted)
                .SumAsync(e => e.Amount);
        }
    }
}

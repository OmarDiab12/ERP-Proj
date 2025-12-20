using ERP.Models.ProjectsManagement;

namespace ERP.Repositories.Interfaces.ProjectsManagement
{
    public interface IProjectExpenseRepository : IBaseRepository<ProjectExpense>
    {
        Task<IEnumerable<ProjectExpense>> GetByProjectIdAsync(int projectId);
        Task<decimal> GetTotalExpensesAsync(int projectId);
    }
}

using ERP.Models.Projects;

namespace ERP.Repositories.Interfaces.ProjectsManagement
{
    public interface IProjectRepository : IBaseRepository<Project>
    {
        Task<Project?> GetFullProjectByIdAsync(int id);
        Task<IEnumerable<Project>> GetActiveProjectsAsync();
        Task<decimal> GetTotalProfitAsync(int projectId);
        // Inside IProjectRepository
        Task SoftDeleteTaskAsync(int taskId, int userId);
        Task SoftDeleteProfitShareAsync(int profitShareId, int userId);
        Task SoftDeletePaymentAsync(int paymentId, int userId);
        Task SoftDeleteExpenseAsync(int expenseId, int userId);

    }
}

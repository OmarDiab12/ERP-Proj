using ERP.Models.ProjectsManagement;

namespace ERP.Repositories.Interfaces.ProjectsManagement
{
    public interface IProjectPaymentRepository  :IBaseRepository<ProjectPayment>
    {
        Task<IEnumerable<ProjectPayment>> GetByProjectIdAsync(int projectId);
        Task<decimal> GetTotalIncomeAsync(int projectId);
    }
}

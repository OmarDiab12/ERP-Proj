using ERP.Models.PrivatePartnerships;

namespace ERP.Repositories.Interfaces.PrivatePartnerships
{
    public interface IPrivatePartnershipProjectRepository : IBaseRepository<PrivatePartnershipProject>
    {
        Task<PrivatePartnershipProject?> GetProjectWithDetailsAsync(int projectId);
        Task<PrivatePartnershipProject> CreateProjectWithSharesAsync(PrivatePartnershipProject project, IEnumerable<PrivatePartnershipPartnerShare> shares, int userId);
        Task<IEnumerable<PrivatePartnershipProject>> GetAllWithDetailsAsync();
    }
}

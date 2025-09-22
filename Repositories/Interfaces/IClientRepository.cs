using ERP.Models.ClientsManagement;

namespace ERP.Repositories.Interfaces
{
    public interface IClientRepository : IBaseRepository<Client>
    {
        Task<List<ClientAccountStatement>> getClientStatements(int clientId);
        Task<List<ClientAccountStatement>> getStatementsforClientperProject(int clientId, int projectId);
    }
}

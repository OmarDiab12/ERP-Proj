using ERP.Models.Brokers;

namespace ERP.Repositories.Interfaces.Persons
{
    public interface IBrokerRepository : IBaseRepository<Broker>
    {
        Task<List<BrokerComission>> GetBrokerCommissions(int brokerId);
        Task<bool> CreateCommisionAsync(BrokerComission brokerComission, int userId);
        Task<IEnumerable<BrokerComission>> GetByProjectIdAsync(int projectId);
        Task<bool> UpdateCommisionAsync(BrokerComission brokerComission, int userId);
        Task<bool> SoftDeleteCommisionAsync(int id, int userId);
    }
}

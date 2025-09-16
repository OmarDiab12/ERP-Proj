using ERP.Models.Brokers;

namespace ERP.Repositories.Interfaces
{
    public interface IBrokerRepository : IBaseRepository<Broker>
    {
        Task<List<BrokerComission>> GetBrokerCommissions(int brokerId);
    }
}

using ERP.Models.Brokers;

namespace ERP.Repositories.Interfaces.Persons
{
    public interface IBrokerRepository : IBaseRepository<Broker>
    {
        Task<List<BrokerComission>> GetBrokerCommissions(int brokerId);
    }
}

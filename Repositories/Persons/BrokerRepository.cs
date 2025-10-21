using ERP.Models;
using ERP.Models.Brokers;
using ERP.Repositories.Interfaces.Persons;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ERP.Repositories.Persons
{
    public class BrokerRepository : BaseRepository<Broker>, IBrokerRepository
    {
        public BrokerRepository(ERPDBContext context) : base(context) { }

        public async Task<List<BrokerComission>> GetBrokerCommissions(int brokerId)
        {
            return await _context.brokerComissions.Where(c=>c.BrokerId == brokerId).ToListAsync();
        }
        

    }
}

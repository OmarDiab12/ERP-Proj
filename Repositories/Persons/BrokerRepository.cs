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

        public async Task<bool> CreateCommisionAsync(BrokerComission brokerComission, int userId)
        {
            try
            {
                brokerComission.CreatedBy = userId;
                brokerComission.CreatedAt = DateTime.UtcNow;
                brokerComission.IsDeleted = false;

                await _context.brokerComissions.AddAsync(brokerComission);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<BrokerComission>> GetByProjectIdAsync(int projectId)
        {
            return await _context.brokerComissions
                .Where(b => b.ProjectId == projectId && !b.IsDeleted)
                .ToListAsync();
        }
    }
}

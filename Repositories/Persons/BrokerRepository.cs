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

        // 🔹 Get all commissions for a broker
        public async Task<List<BrokerComission>> GetBrokerCommissions(int brokerId)
        {
            return await _context.brokerComissions
                .Where(c => c.BrokerId == brokerId && !c.IsDeleted)
                .ToListAsync();
        }

        // 🔹 Create a new commission
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

        // 🔹 Update an existing commission
        public async Task<bool> UpdateCommisionAsync(BrokerComission brokerComission, int userId)
        {
            try
            {
                var existing = await _context.brokerComissions
                    .FirstOrDefaultAsync(b => b.Id == brokerComission.Id && !b.IsDeleted);

                if (existing == null)
                    return false;

                existing.BrokerId = brokerComission.BrokerId;
                existing.ProjectId = brokerComission.ProjectId;
                existing.PercentofTotal = brokerComission.PercentofTotal;

                existing.UpdatedBy = userId;
                existing.UpdatedAt = DateTime.UtcNow;

                _context.brokerComissions.Update(existing);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // 🔹 Soft delete a single commission by ID
        public async Task<bool> SoftDeleteCommisionAsync(int id, int userId)
        {
            try
            {
                var entity = await _context.brokerComissions
                    .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

                if (entity == null)
                    return false;

                entity.IsDeleted = true;
                entity.UpdatedBy = userId;
                entity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // 🔹 Soft delete all commissions by project ID
        public async Task<bool> SoftDeleteByProjectIdAsync(int projectId, int userId)
        {
            try
            {
                var list = await _context.brokerComissions
                    .Where(b => b.ProjectId == projectId && !b.IsDeleted)
                    .ToListAsync();

                if (!list.Any())
                    return true;

                foreach (var c in list)
                {
                    c.IsDeleted = true;
                    c.UpdatedBy = userId;
                    c.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // 🔹 Get all commissions linked to a project
        public async Task<IEnumerable<BrokerComission>> GetByProjectIdAsync(int projectId)
        {
            return await _context.brokerComissions
                .Where(b => b.ProjectId == projectId && !b.IsDeleted)
                .ToListAsync();
        }
    }
}


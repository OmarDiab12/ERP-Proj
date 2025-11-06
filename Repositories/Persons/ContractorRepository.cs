using ERP.Models;
using ERP.Models.ContractorsManagement;
using ERP.Repositories.Interfaces.Persons;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ERP.Repositories.Persons
{
    public class ContractorRepository : BaseRepository<Contractor>, IContractorRepository
    {
        public ContractorRepository(ERPDBContext context) : base(context) { }
        
        public async Task<List<ContractOfContractor>> GetContractorContracts(int contractorId)
        {
            return await _context.ContractOfContracts.Where(c=>c.ContractorId == contractorId).ToListAsync();
        }
        public async Task<ContractOfContractor> CreateContractAsync(ContractOfContractor contract, int userId)
        {
            try
            {
                contract.CreatedBy = userId;
                contract.CreatedAt = DateTime.UtcNow;
                contract.IsDeleted = false;

                await _context.ContractOfContracts.AddAsync(contract);
                await _context.SaveChangesAsync();
                return contract ;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CreateContractPaymentsAsync(List<ContactPayment> payments, int userId)
        {
            try
            {
                foreach (var payment in payments)
                {
                    payment.CreatedBy = userId;
                    payment.CreatedAt = DateTime.UtcNow;
                    payment.IsDeleted = false;
                }

                await _context.ContactPayments.AddRangeAsync(payments);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<IEnumerable<ContractOfContractor>> GetByProjectIdAsync(int projectId)
        {
            return await _context.ContractOfContracts
                .Where(c => c.ProjectId == projectId && !c.IsDeleted)
                .ToListAsync();
        }


    }
}

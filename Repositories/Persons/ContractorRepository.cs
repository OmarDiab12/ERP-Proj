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
    }
}

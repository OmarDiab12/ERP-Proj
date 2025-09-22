using ERP.Models.ContractorsManagement;

namespace ERP.Repositories.Interfaces
{
    public interface IContractorRepository : IBaseRepository<Contractor>
    {
        Task<List<ContractOfContractor>> GetContractorContracts(int contractorId);
    }
}

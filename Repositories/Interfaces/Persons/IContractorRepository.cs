using ERP.Models.ContractorsManagement;

namespace ERP.Repositories.Interfaces.Persons
{
    public interface IContractorRepository : IBaseRepository<Contractor>
    {
        Task<List<ContractOfContractor>> GetContractorContracts(int contractorId);
    }
}

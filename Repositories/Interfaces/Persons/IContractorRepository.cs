using ERP.Models.ContractorsManagement;

namespace ERP.Repositories.Interfaces.Persons
{
    public interface IContractorRepository : IBaseRepository<Contractor>
    {
        Task<List<ContractOfContractor>> GetContractorContracts(int contractorId);
        Task<bool> CreateContractAsync(ContractOfContractor contract, int userId);
        Task<IEnumerable<ContractOfContractor>> GetByProjectIdAsync(int projectId);
    }
}

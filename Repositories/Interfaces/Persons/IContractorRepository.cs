using ERP.Models.ContractorsManagement;

namespace ERP.Repositories.Interfaces.Persons
{
    public interface IContractorRepository : IBaseRepository<Contractor>
    {
        Task<List<ContractOfContractor>> GetContractorContracts(int contractorId);
        Task<ContractOfContractor> CreateContractAsync(ContractOfContractor contract, int userId);
        Task<IEnumerable<ContractOfContractor>> GetByProjectIdAsync(int projectId);
        Task<bool> CreateContractPaymentsAsync(List<ContactPayment> payments, int userId);
    }
}

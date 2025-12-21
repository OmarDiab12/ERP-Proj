using ERP.Models.ContractorsManagement;

namespace ERP.Repositories.Interfaces.Persons
{
    public interface IContractorRepository : IBaseRepository<Contractor>
    {
        Task<ContractOfContractor> CreateContractAsync(ContractOfContractor contract, int userId);
        Task<ContractOfContractor?> UpdateContractAsync(ContractOfContractor contract, int userId);        
        Task<bool> DeleteContractAsync(int contractId, int userId);
        Task<List<ContractOfContractor>> GetByContractorIdAsync(int contractorId);
        Task<List<ContractOfContractor>> GetByProjectIdAsync(int projectId);
        Task<List<ContractOfContractor>> GetByProjectAndContractorAsync(int projectId, int contractorId);
        Task SyncContractsAsync(List<ContractOfContractor> incomingContracts, int projectId, int userId);
        Task<bool> CreateContractPaymentsAsync(List<ContactPayment> payments, int userId);
        Task<List<ContactPayment>> GetContractPaymentsAsync(int contractId);
        Task<bool> DeletePaymentAsync(int paymentId, int userId);
        Task SyncPaymentsAsync(List<ContactPayment> incomingPayments, int contractId, int userId);
        Task<List<ContractOfContractor>> GetAllContracts();
    }
}

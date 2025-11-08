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

        // ================= CONTRACTS ====================

        public async Task<ContractOfContractor> CreateContractAsync(ContractOfContractor contract, int userId)
        {
            contract.CreatedBy = userId;
            contract.CreatedAt = DateTime.UtcNow;
            contract.IsDeleted = false;

            await _context.ContractOfContracts.AddAsync(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task<ContractOfContractor?> UpdateContractAsync(ContractOfContractor contract, int userId)
        {
            var existing = await _context.ContractOfContracts
                .FirstOrDefaultAsync(c => c.Id == contract.Id && !c.IsDeleted);

            if (existing == null) return null;

            existing.ContractorId = contract.ContractorId;
            existing.ProjectId = contract.ProjectId;
            existing.ContractAmount = contract.ContractAmount;
            existing.Description = contract.Description;
            existing.StartDate = contract.StartDate;
            existing.EndDate = contract.EndDate;
            existing.UpdatedBy = userId;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.ContractOfContracts.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteContractAsync(int contractId, int userId)
        {
            var contract = await _context.ContractOfContracts.FirstOrDefaultAsync(c => c.Id == contractId);
            if (contract == null) return false;

            contract.IsDeleted = true;
            contract.UpdatedBy = userId;
            contract.UpdatedAt = DateTime.UtcNow;

            _context.ContractOfContracts.Update(contract);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ContractOfContractor>> GetByContractorIdAsync(int contractorId)
        {
            return await _context.ContractOfContracts
                .Where(c => c.ContractorId == contractorId && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<ContractOfContractor>> GetByProjectIdAsync(int projectId)
        {
            return await _context.ContractOfContracts
                .Where(c => c.ProjectId == projectId && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<ContractOfContractor>> GetByProjectAndContractorAsync(int projectId, int contractorId)
        {
            return await _context.ContractOfContracts
                .Where(c => c.ProjectId == projectId && c.ContractorId == contractorId && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task SyncContractsAsync(List<ContractOfContractor> incomingContracts, int projectId, int userId)
        {
            var existingContracts = await GetByProjectIdAsync(projectId);

            // Delete missing
            var incomingIds = incomingContracts.Where(c => c.Id != 0).Select(c => c.Id).ToList();
            var toDelete = existingContracts.Where(c => !incomingIds.Contains(c.Id));
            foreach (var del in toDelete)
            {
                await DeleteContractAsync(del.Id, userId);
            }

            // Insert or update
            foreach (var contract in incomingContracts)
            {
                if (contract.Id == 0)
                    await CreateContractAsync(contract, userId);
                else
                    await UpdateContractAsync(contract, userId);
            }
        }

        // =============== CONTRACT PAYMENTS =====================

        public async Task<bool> CreateContractPaymentsAsync(List<ContactPayment> payments, int userId)
        {
            foreach (var p in payments)
            {
                p.CreatedBy = userId;
                p.CreatedAt = DateTime.UtcNow;
                p.IsDeleted = false;
            }

            await _context.ContactPayments.AddRangeAsync(payments);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ContactPayment>> GetContractPaymentsAsync(int contractId)
        {
            return await _context.ContactPayments
                .Where(p => p.ContractId == contractId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> DeletePaymentAsync(int paymentId, int userId)
        {
            var payment = await _context.ContactPayments.FirstOrDefaultAsync(p => p.Id == paymentId);
            if (payment == null) return false;

            payment.IsDeleted = true;
            payment.UpdatedBy = userId;
            payment.UpdatedAt = DateTime.UtcNow;

            _context.ContactPayments.Update(payment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SyncPaymentsAsync(List<ContactPayment> incomingPayments, int contractId, int userId)
        {
            var existing = await GetContractPaymentsAsync(contractId);
            var incomingIds = incomingPayments.Where(p => p.Id != 0).Select(p => p.Id).ToList();

            var toDelete = existing.Where(p => !incomingIds.Contains(p.Id));
            foreach (var del in toDelete)
            {
                await DeletePaymentAsync(del.Id, userId);
            }

            foreach (var p in incomingPayments)
            {
                if (p.Id == 0)
                    await CreateContractPaymentsAsync(new List<ContactPayment> { p }, userId);
                else
                {
                    var existingPayment = existing.FirstOrDefault(e => e.Id == p.Id);
                    if (existingPayment != null)
                    {
                        existingPayment.amount = p.amount;
                        existingPayment.status = p.status;
                        existingPayment.dateTime = p.dateTime;
                        existingPayment.UpdatedBy = userId;
                        existingPayment.UpdatedAt = DateTime.UtcNow;

                        _context.ContactPayments.Update(existingPayment);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }

}

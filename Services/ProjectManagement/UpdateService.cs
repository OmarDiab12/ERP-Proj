using ERP.DTOs.Projects;
using ERP.Helpers;
using ERP.Models.Brokers;
using ERP.Models.ContractorsManagement;
using ERP.Models.ProjectsManagement;
using ERP.Repositories.Interfaces.Persons;
using ERP.Repositories.Interfaces.ProjectsManagement;
using ERP.Services.Interfaces.ProjectManagement;
using System.Transactions;


namespace ERP.Services.ProjectManagement
{

    public class UpdateService : IUpdateService
    {
        private readonly IProjectRepository _projectRepo;
        private readonly IProjectAttachmentRepository _attachmentRepo;
        private readonly IBrokerRepository _brokerComRepo;
        private readonly IContractorRepository _contractRepo;
        private readonly IFileStorageService _fileStorage;
        private readonly IErrorRepository _errors;

        public UpdateService(
            IProjectRepository projectRepo,
            IProjectAttachmentRepository attachmentRepo,
            IBrokerRepository brokerComRepo,
            IContractorRepository contractRepo,
            IFileStorageService fileStorage,
            IErrorRepository errors)
        {
            _projectRepo = projectRepo;
            _attachmentRepo = attachmentRepo;
            _brokerComRepo = brokerComRepo;
            _contractRepo = contractRepo;
            _fileStorage = fileStorage;
            _errors = errors;
        }

        public async Task<ResponseDTO> UpdateProjectAsync(ProjectUpdateFullDTO dto, int userId)
        {
            const string fn = nameof(UpdateProjectAsync);
            try
            {
                // Begin a transaction with async flow support
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // 🔹 1. Validate project existence
                var project = await _projectRepo.GetByIdAsync(dto.Id);
                if (project == null)
                    return new ResponseDTO { IsValid = false, Message = "Project not found" };

                // 🔹 2. Validate dates
                if (!DateTime.TryParse(dto.StartDate, out var startDate))
                    return new ResponseDTO { IsValid = false, Message = "Invalid StartDate" };

                DateTime? endDate = null;
                if (!string.IsNullOrWhiteSpace(dto.EndDate))
                {
                    if (!DateTime.TryParse(dto.EndDate, out var parsedEnd))
                        return new ResponseDTO { IsValid = false, Message = "Invalid EndDate" };
                    endDate = parsedEnd;
                }

                // 🔹 3. Validate status enum
                if (!Enum.IsDefined(typeof(ProjectStatus), dto.Status))
                    return new ResponseDTO { IsValid = false, Message = "Invalid project status value" };

                // 🔹 4. Update base project info
                project.ProjectName = dto.ProjectName;
                project.Description = dto.Description;
                project.location = dto.Location;
                project.StartDate = startDate;
                project.EndDate = endDate;
                project.Status =  dto.Status;
                project.BrokerId = dto.BrokerId;

                await _projectRepo.UpdateAsync(project, userId);

                // 🔹 5. Handle Broker Commission
                if (dto.BrokerId.HasValue && dto.BrokerCommissionPercentage.HasValue)
                {
                    var existingBroker = project.BrokerComissions.FirstOrDefault();
                    if (existingBroker != null)
                    {
                        existingBroker.BrokerId = dto.BrokerId.Value;
                        existingBroker.PercentofTotal = dto.BrokerCommissionPercentage.Value;
                        await _brokerComRepo.UpdateCommisionAsync(existingBroker, userId);
                    }
                    else
                    {
                        await _brokerComRepo.CreateCommisionAsync(new BrokerComission
                        {
                            ProjectId = project.Id,
                            BrokerId = dto.BrokerId.Value,
                            PercentofTotal = dto.BrokerCommissionPercentage.Value
                        }, userId);
                    }
                }

                // 🔹 6. Handle Contractors (full sync)
                var contractEntities = new List<ContractOfContractor>();
                foreach (var c in dto.Contractors)
                {
                    if (!DateTime.TryParse(c.ContractStartDate, out var cStart))
                        return new ResponseDTO { IsValid = false, Message = $"Invalid ContractStartDate for contractor {c.ContractorId}" };

                    if (!DateTime.TryParse(c.ContractEndDate, out var cEnd))
                        return new ResponseDTO { IsValid = false, Message = $"Invalid ContractEndDate for contractor {c.ContractorId}" };

                    var contract = new ContractOfContractor
                    {
                        Id = c.Id,
                        ProjectId = project.Id,
                        ContractorId = c.ContractorId,
                        ContractAmount = c.ContractAmount,
                        Description = c.Description,
                        StartDate = cStart,
                        EndDate = cEnd
                    };

                    contractEntities.Add(contract);
                }

                await _contractRepo.SyncContractsAsync(contractEntities, project.Id, userId);

                // 🔹 7. Handle Payments (full sync per contractor)
                foreach (var c in dto.Contractors)
                {
                    var payments = new List<ContactPayment>();
                    foreach (var p in c.Payments)
                    {
                        if (!DateTime.TryParse(p.PaymentDate, out var payDate))
                            return new ResponseDTO { IsValid = false, Message = $"Invalid PaymentDate for contractor {c.ContractorId}" };

                        if (!Enum.TryParse<PaymentStatus>(p.Status, true, out var parsedStatus))
                            return new ResponseDTO { IsValid = false, Message = $"Invalid PaymentStatus for contractor {c.ContractorId}" };

                        payments.Add(new ContactPayment
                        {
                            Id = p.Id,
                            amount = p.Amount,
                            status = parsedStatus,
                            dateTime = payDate,
                            ContractId = c.Id
                        });
                    }

                    await _contractRepo.SyncPaymentsAsync(payments, c.Id, userId);
                }

                // 🔹 8. Handle Attachments (add/remove)
                foreach (var path in dto.RemoveAttachmentPaths)
                {
                    var att = project.ProjectAttachments.FirstOrDefault(a => a.FilePath == path);
                    if (att != null)
                    {
                        await _fileStorage.DeleteFileAsync(att.FilePath);
                        await _attachmentRepo.SoftDeleteAsync(att.Id, userId);
                    }
                }

                foreach (var file in dto.NewAttachments)
                {
                    var result = await _fileStorage.SaveFileAsync(file, $"Projects/{project.Id}");
                    if (result != null)
                    {
                        await _attachmentRepo.CreateAsync(new ProjectAttachment
                        {
                            ProjectId = project.Id,
                            FileName = result.FileName,
                            FilePath = result.FilePath
                        }, userId);
                    }
                }

                // 🔹 9. Commit transaction
                scope.Complete();

                return new ResponseDTO { IsValid = true, Message = "Project updated successfully" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while updating project" };
            }
        }

        public async Task<ResponseDTO> DeleteProjectAsync(int projectId, int userId)
        {
            const string fn = nameof(DeleteProjectAsync);
            try
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // 🔹 1. Get project
                var project = await _projectRepo.GetByIdAsync(projectId);
                if (project == null)
                    return new ResponseDTO { IsValid = false, Message = "Project not found" };

                // 🔹 2. Soft delete all related entities

                // Attachments
                if (project.ProjectAttachments != null && project.ProjectAttachments.Any())
                {
                    foreach (var att in project.ProjectAttachments)
                    {
                        await _fileStorage.DeleteFileAsync(att.FilePath);
                        await _attachmentRepo.SoftDeleteAsync(att.Id, userId);
                    }
                }

                // Broker Commissions
                if (project.BrokerComissions != null && project.BrokerComissions.Any())
                {
                    foreach (var b in project.BrokerComissions)
                        await _brokerComRepo.SoftDeleteCommisionAsync(b.Id, userId);
                }

                // Contractors & their contracts
                if (project.ContractOfContracts != null && project.ContractOfContracts.Any())
                {
                    foreach (var contract in project.ContractOfContracts)
                    {
                        // Payments related to this contract
                        var payments = await _contractRepo.GetContractPaymentsAsync(contract.Id);
                        foreach (var pay in payments)
                            await _contractRepo.DeletePaymentAsync(pay.Id, userId);

                        await _contractRepo.DeleteContractAsync(contract.Id, userId);
                    }
                }

                // Tasks
                if (project.ProjectTasks != null && project.ProjectTasks.Any())
                {
                    foreach (var task in project.ProjectTasks)
                        await _projectRepo.SoftDeleteTaskAsync(task.Id, userId);
                }

                // Profit shares
                if (project.ProjectProfitShares != null && project.ProjectProfitShares.Any())
                {
                    foreach (var ps in project.ProjectProfitShares)
                        await _projectRepo.SoftDeleteProfitShareAsync(ps.Id, userId);
                }

                // Payments & Expenses (just in case)
                if (project.ProjectPayments != null && project.ProjectPayments.Any())
                {
                    foreach (var pay in project.ProjectPayments)
                        await _projectRepo.SoftDeletePaymentAsync(pay.Id, userId);
                }

                if (project.ProjectExpenses != null && project.ProjectExpenses.Any())
                {
                    foreach (var exp in project.ProjectExpenses)
                        await _projectRepo.SoftDeleteExpenseAsync(exp.Id, userId);
                }

                // 🔹 3. Finally, soft delete the project itself
                await _projectRepo.SoftDeleteAsync(project.Id, userId);

                // 🔹 4. Commit transaction
                scope.Complete();

                return new ResponseDTO { IsValid = true, Message = "Project and all related data deleted successfully." };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while deleting project." };
            }
        }

    }

}


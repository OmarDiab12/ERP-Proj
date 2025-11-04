using ERP.DTOs.Projects;
using ERP.Helpers;
using ERP.Models.Brokers;
using ERP.Models.ContractorsManagement;
using ERP.Models.Projects;
using ERP.Models.ProjectsManagement;
using ERP.Repositories.Interfaces.Persons;
using ERP.Repositories.Interfaces.ProjectsManagement;
using ERP.Repositories.Interfaces.QuotationManagement;
using ERP.Services.Interfaces.ProjectManagement;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace ERP.Services.ProjectManagement
{
    public class CreateService : ICreateService
    {
        private readonly IProjectRepository _projectRepo;
        private readonly IProjectAttachmentRepository _attachmentRepo;
        private readonly IBrokerRepository _brokerComRepo;
        private readonly IContractorRepository _contractRepo;
        private readonly IQuotationRepository _quotationRepo;
        private readonly IFileStorageService _fileStorage;
        private readonly IErrorRepository _errors;

        public CreateService(
            IProjectRepository projectRepo,
            IProjectAttachmentRepository attachmentRepo,
            IBrokerRepository brokerComRepo,
            IContractorRepository contractRepo,
            IQuotationRepository quotationRepo,
            IFileStorageService fileStorage,
            IErrorRepository errors)
        {
            _projectRepo = projectRepo;
            _attachmentRepo = attachmentRepo;
            _brokerComRepo = brokerComRepo;
            _contractRepo = contractRepo;
            _quotationRepo = quotationRepo;
            _fileStorage = fileStorage;
            _errors = errors;
        }

        // ===========================================================
        // CREATE PROJECT BASED ON ACCEPTED QUOTATION
        // ===========================================================
        public async Task<ResponseDTO> CreateFromQuotationAsync(int quotationId, int userId)
        {
            const string fn = nameof(CreateFromQuotationAsync);
            try
            {
                var quotation = await _quotationRepo.GetByIdAsync(quotationId);
                if (quotation == null)
                    return new ResponseDTO { IsValid = false, Message = "Quotation not found" };

                // Prevent duplicate projects
                var existing = _projectRepo.Query().FirstOrDefault(p => p.quotationId == quotationId && !p.IsDeleted);
                if (existing != null)
                    return new ResponseDTO { IsValid = false, Message = "Project already exists for this quotation" };

                // Create project entity
                var project = new Project
                {
                    quotationId = quotation.Id,
                    ClientId = quotation.ClientId,
                    ProjectName = $"Project for Quotation {quotation.Client.Name}",
                    Description = quotation.GeneralNotes ?? "Auto-created from quotation",
                    Status = ProjectStatus.InProgress,
                    StartDate = DateTime.UtcNow,
                    TotalCost = quotation.TotalAmount
                };

                await _projectRepo.CreateAsync(project, userId);

                return new ResponseDTO
                {
                    IsValid = true,
                    Data = project.Id,
                    Message = "Project created successfully from quotation."
                };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while creating project." };
            }
        }

        // ===========================================================
        // MANUAL PROJECT CREATION (From UI Wizard)
        // ===========================================================
        public async Task<ResponseDTO> CreateFullProjectAsync(ProjectCreateFullDTO dto, int userId)
        { 
            const string fn = nameof(CreateFullProjectAsync);
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                // Step 1: Create Project
                var project = new Project
                {
                    ProjectName = dto.ProjectName,
                    Description = dto.Description,
                    ClientId = dto.ClientId,
                    location = dto.Location,
                    BrokerId = dto.BrokerId,
                    quotationId = dto.QuotationId ?? 0,
                    StartDate = DateTime.Parse(dto.StartDate),
                    Status = dto.Status
                };

                await _projectRepo.CreateAsync(project, userId);

                // Step 2: Save Attachments
                foreach (var file in dto.Attachments)
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

                // Step 3: Broker Commission
                if (dto.BrokerId.HasValue && dto.BrokerCommissionPercentage.HasValue)
                {
                    await _brokerComRepo.CreateCommisionAsync(new BrokerComission
                    {
                        BrokerId = dto.BrokerId.Value,
                        ProjectId = project.Id,
                        PercentofTotal = dto.BrokerCommissionPercentage.Value,
                        CreatedAt = DateTime.UtcNow
                    }, userId);
                }

                // Step 4: Contractors
                foreach (var c in dto.Contractors)
                {
                    await _contractRepo.CreateContractAsync(new ContractOfContractor
                    {
                        ProjectId = project.Id,
                        ContractorId = c.ContractorId,
                        ContractAmount = c.ContractAmount,
                        Description = c.ContractDescription,
                        StartDate = c.ContractStartDate ?? DateTime.UtcNow,
                        EndDate = c.ContractEndDate ?? DateTime.UtcNow.AddDays(365)
                    }, userId);
                }

                // ✅ Commit transaction
                scope.Complete();

                return new ResponseDTO
                {
                    IsValid = true,
                    Data = project.Id,
                    Message = "Project created successfully with all related data."
                };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while creating project." };
            }
        }
    }
}



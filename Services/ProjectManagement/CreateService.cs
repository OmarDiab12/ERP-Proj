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

            // 🔹 PRE-VALIDATION PHASE — No DB writes yet
            if (!DateTime.TryParse(dto.StartDate, out var projectStart))
                return new ResponseDTO { IsValid = false, Message = "Invalid StartDate" };

            DateTime? projectEnd = null;
            if (!string.IsNullOrWhiteSpace(dto.EndDate))
            {
                if (!DateTime.TryParse(dto.EndDate, out var parsedEnd))
                    return new ResponseDTO { IsValid = false, Message = "Invalid EndDate" };
                projectEnd = parsedEnd;
            }

            // 🔹 Validate Contractors and their Details
            foreach (var contractor in dto.Contractors)
            {
                if (!DateTime.TryParse(contractor.ContractStartDate, out var cStart))
                    return new ResponseDTO { IsValid = false, Message = $"Invalid ContractStartDate for contractor ID {contractor.ContractorId}" };

                if (!DateTime.TryParse(contractor.ContractEndDate, out var cEnd))
                    return new ResponseDTO { IsValid = false, Message = $"Invalid ContractEndDate for contractor ID {contractor.ContractorId}" };

                if (cEnd < cStart)
                    return new ResponseDTO { IsValid = false, Message = $"EndDate cannot be earlier than StartDate for contractor ID {contractor.ContractorId}" };

                // 🔹 Validate ContractDetails inside each contractor
                foreach (var detail in contractor.contractDetails)
                {
                    if (!DateTime.TryParse(detail.dateTime, out _))
                        return new ResponseDTO
                        {
                            IsValid = false,
                            Message = $"Invalid dateTime in contract detail (index {detail.index}) for contractor ID {contractor.ContractorId}"
                        };

                    // ✅ Validate Status (string → enum)
                    if (!Enum.TryParse<PaymentStatus>(detail.status, true, out var _))
                        return new ResponseDTO
                        {
                            IsValid = false,
                            Message = $"Invalid status '{detail.status}' in contract detail (index {detail.index}) for contractor ID {contractor.ContractorId}"
                        };
                }

            }

            // 🔹 Validate Broker Commission
            if (dto.BrokerCommissionPercentage.HasValue)
            {
                if (dto.BrokerCommissionPercentage.Value < 0 || dto.BrokerCommissionPercentage.Value > 100)
                    return new ResponseDTO { IsValid = false, Message = "Broker commission percentage must be between 0 and 100." };
            }

            // 🔹 Validate Contract Amounts
            foreach (var contractor in dto.Contractors)
            {
                if (contractor.ContractAmount <= 0)
                    return new ResponseDTO { IsValid = false, Message = $"ContractAmount must be greater than 0 for contractor ID {contractor.ContractorId}" };
            }

            // 🔹 Start Transaction
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                // STEP 1️⃣ — Create Project
                var project = new Project
                {
                    ProjectName = dto.ProjectName,
                    Description = dto.Description,
                    ClientId = dto.ClientId,
                    location = dto.Location,
                    BrokerId = dto.BrokerId,
                    quotationId = dto.QuotationId ?? 0,
                    StartDate = projectStart,
                    EndDate = projectEnd,
                    Status = dto.Status
                };

                await _projectRepo.CreateAsync(project, userId);

                // STEP 2️⃣ — Save Attachments (if any)
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

                // STEP 3️⃣ — Broker Commission
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

                // STEP 4️⃣ — Contractors and their Payments
                foreach (var c in dto.Contractors)
                {
                    // safely parse now (we already validated)
                    DateTime.TryParse(c.ContractStartDate, out var cStart);
                    DateTime.TryParse(c.ContractEndDate, out var cEnd);

                    var contract = new ContractOfContractor
                    {
                        ProjectId = project.Id,
                        ContractorId = c.ContractorId,
                        ContractAmount = c.ContractAmount,
                        Description = c.ContractDescription ?? string.Empty,
                        StartDate = cStart,
                        EndDate = cEnd
                    };

                    await _contractRepo.CreateContractAsync(contract, userId);

                    // nested contract details (if any)
                    if (c.contractDetails.Any())
                    {
                        var payments = c.contractDetails.Select(d => new ContactPayment
                        {
                            ContractId = contract.Id,
                            amount = d.amount,
                            index = d.index,
                            status = Enum.TryParse<PaymentStatus>(d.status, true, out var parsedStatus) ? parsedStatus : PaymentStatus.Pending,
                            dateTime = DateTime.Parse(d.dateTime)
                        }).ToList();

                        await _contractRepo.CreateContractPaymentsAsync(payments, userId);
                    }
                }

                // ✅ Commit all changes
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



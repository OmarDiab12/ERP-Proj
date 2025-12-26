using ERP.DTOs.Projects;
using ERP.Models.Brokers;
using ERP.Models.Projects;
using ERP.Repositories.Interfaces.ProjectsManagement;
using ERP.Services.Interfaces.ProjectManagement;

namespace ERP.Services.ProjectManagement
{
    public class GetService : IGetService
    {
        private readonly IProjectRepository _projectRepo;
        private readonly IErrorRepository _errors;

        public GetService(IProjectRepository projectRepository, IErrorRepository errors)
        {
            _projectRepo = projectRepository;
            _errors = errors;
        }

        // =====================================================
        // GET BY ID — Full Details
        // =====================================================
        public async Task<ResponseDTO> GetByIdAsync(int id)
        {
            const string fn = nameof(GetByIdAsync);
            try
            {
                var project = await _projectRepo.GetByIdAsync(id);
                if (project == null)
                    return new ResponseDTO { IsValid = false, Message = "Project not found" };

                var dto = new ProjectDetailDto
                {
                    Id = project.Id,
                    ProjectName = project.ProjectName,
                    Description = project.Description,
                    Clientid = project.ClientId ?? 0,
                    ClientName = project.Client?.Name ?? "N/A",
                    brokerid = project.BrokerId ?? 0,
                    brokerpercent = project.BrokerComissions.Where(c => c.BrokerId == project.BrokerId && c.ProjectId == project.Id).Select(c=>c.PercentofTotal).FirstOrDefault(),
                    BrokerName = project.Broker?.Name ?? "N/A",
                    Status = project.Status.ToString(),
                    StartDate = project.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = project.EndDate?.ToString("yyyy-MM-dd"),
                    Location = project.location,
                    TotalPayments = project.TotalPaymentsReceived,
                    TotalExpenses = project.TotalExpenses,
                    TotalContractorPayments = project.TotalContractorPayments,
                    NetProfit = project.NetProfit,
                    Attachments = project.ProjectAttachments
                        .Select(a => new ProjectAttachmentDto
                        {
                            FileName = a.FileName,
                            FilePath = a.FilePath
                        }).ToList(),
                    Contractors = project.ContractOfContracts
                            .Select(c => new ProjectContractorDto
                            {
                                Id = c.Id,
                                ContractorName = c.Contractor?.Name ?? "N/A",
                                ContractAmount = c.ContractAmount,
                                Description = c.Description,
                                StartDate = c.StartDate.ToString("yyyy-MM-dd"),
                                EndDate = c.EndDate == default ? null : c.EndDate.ToString("yyyy-MM-dd"),
                                Payments = c.ContactPayments?.Select(p => new ContractPaymentDto
                                {
                                Id = p.Id,
                                Amount = p.amount,
                                Status = p.status.ToString(),
                                index = p.index,
                                PaymentDate = p.dateTime.ToString("yyyy-MM-dd")
                                    }).ToList() ?? new List<ContractPaymentDto>()
                            }).ToList(),
                    Tasks = project.ProjectTasks
                        .Select(t => new ProjectTaskDto
                        {
                            Id = t.Id,
                            ProjectId = project.Id,
                            Title = t.Title,
                            Description = t.Description,
                            TaskType = t.TaskType,
                            Priority = t.Priority,
                            Status = t.Status,
                            ReferenceType = t.ReferenceType,
                            ReferenceId = t.ReferenceId,
                            StartDate = t.StartDate.ToString("yyyy-MM-dd"),
                            DueDate = t.DueDate?.ToString("yyyy-MM-dd"),
                            AssignedToEmployeeId = t.AssignedToId,
                            AssignedPartnerId = t.AssignedPartnerId,
                            AssignedToName = t.AssignedTo != null
                                ? t.AssignedTo.Name
                                : t.AssignedPartner != null
                                    ? t.AssignedPartner.FullName
                                    : null
                        }).ToList()
                };

                return new ResponseDTO { IsValid = true, Data = dto };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while fetching project" };
            }
        }

        // =====================================================
        // GET ALL OR FILTERED
        // =====================================================
        public async Task<ResponseDTO> GetAllAsync(ProjectFilterDto filter)
        {
            const string fn = nameof(GetAllAsync);
            try
            {
                // start with queryable to allow chaining (better performance)
                var query = (await _projectRepo.GetAllAsync()).AsQueryable();

                // Apply filters dynamically
                if (filter.ClientId.HasValue)
                    query = query.Where(c => c.ClientId == filter.ClientId.Value);

                if (filter.BrokerId.HasValue)
                    query = query.Where(c => c.BrokerId == filter.BrokerId.Value);

                if (filter.Status.HasValue)
                    query = query.Where(c => c.Status == filter.Status.Value);

                if (!string.IsNullOrWhiteSpace(filter.From) && !string.IsNullOrWhiteSpace(filter.To)
                    && DateTime.TryParse(filter.From, out var from)
                    && DateTime.TryParse(filter.To, out var to))
                {
                    query = query.Where(c => c.StartDate >= from && c.StartDate <= to);
                }

                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                {
                    var keyword = filter.Keyword.ToLower();
                    query = query.Where(c =>
                        (!string.IsNullOrEmpty(c.ProjectName) && c.ProjectName.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(c.Description) && c.Description.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(c.location) && c.location.ToLower().Contains(keyword)));
                }

                // Now project to DTO
                var list = query.Select(p => new ProjectListDto
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    ClientName = p.Client != null ? p.Client.Name : "N/A",
                    BrokerName = p.Broker != null ? p.Broker.Name : "N/A",
                    Status = p.Status.ToString(),
                    StartDate = p.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = p.EndDate.HasValue ? p.EndDate.Value.ToString("yyyy-MM-dd") : null,
                    TotalPayments = p.TotalPaymentsReceived,
                    TotalExpenses = p.TotalExpenses,
                    TotalContractorPayments = p.TotalContractorPayments,
                    NetProfit = p.NetProfit
                })
                .OrderByDescending(p => p.Id)
                .ToList();

                return new ResponseDTO { IsValid = true, Data = list };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error while fetching projects" };
            }
        }

    }
}

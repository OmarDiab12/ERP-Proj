namespace ERP.DTOs.Projects
{
    public class ProjectCreateFullDTO
    {
        // ========== BASIC INFO ==========
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public int ClientId { get; set; }
        public string Location { get; set; }
        public string StartDate { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
        public string? EndDate { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.InProgress;

        // ========== OPTIONAL LINKS ==========
        public int? QuotationId { get; set; }
        public int? BrokerId { get; set; }
        public decimal? BrokerCommissionPercentage { get; set; }

        // ========== CONTRACTORS ==========
        public List<ProjectContractorDTO> Contractors { get; set; } = new();

        // ========== FILES ==========
        public List<IFormFile> Attachments { get; set; } = new();
    }

    public class ProjectContractorDTO
    {
        public int ContractorId { get; set; }
        public decimal ContractAmount { get; set; }
        public string ContractDescription { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
    }

    public class ProjectListDto
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string ClientName { get; set; }
        public string BrokerName { get; set; }
        public string Status { get; set; }
        public string StartDate { get; set; }
        public string? EndDate { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalContractorPayments { get; set; }
        public decimal NetProfit { get; set; }
    }

    public class ProjectDetailDto : ProjectListDto
    {
        public string Location { get; set; }
        public List<ProjectAttachmentDto> Attachments { get; set; } = new();
        public List<ProjectContractorDto> Contractors { get; set; } = new();
        public List<ProjectTaskDto> Tasks { get; set; } = new();
    }

    public class ProjectContractorDto
    {
        public int Id { get; set; }
        public string ContractorName { get; set; }
        public decimal ContractAmount { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string? EndDate { get; set; }
    }

    public class ProjectAttachmentDto
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public class ProjectFilterDto
    {
        public int? ClientId { get; set; }
        public int? BrokerId { get; set; }
        public ProjectStatus? Status { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Keyword { get; set; }
    }

    public class ProjectTaskDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskPriority Priority { get; set; }
        public Helpers.Enums.TaskStatus Status { get; set; }
        public string StartDate { get; set; }
        public string? DueDate { get; set; }
        public string? AssignedToName { get; set; }
    }
}

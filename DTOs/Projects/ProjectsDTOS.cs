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
}

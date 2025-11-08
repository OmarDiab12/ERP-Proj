namespace ERP.DTOs.Projects
{
    public class ProjectUpdateFullDTO
    {
        // ========== BASIC INFO ==========
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string StartDate { get; set; }
        public string? EndDate { get; set; }
        public ProjectStatus Status { get; set; }

        // ========== BROKER ==========
        public int? BrokerId { get; set; }
        public decimal? BrokerCommissionPercentage { get; set; }

        // ========== CONTRACTORS ==========
        public List<ProjectContractorUpdateDTO> Contractors { get; set; } = new();

        // ========== FILES ==========
        public List<IFormFile> NewAttachments { get; set; } = new();
        public List<string> RemoveAttachmentPaths { get; set; } = new();
    }

    public class ProjectContractorUpdateDTO
    {
        public int Id { get; set; } // 0 => new
        public int ContractorId { get; set; }
        public decimal ContractAmount { get; set; }
        public string Description { get; set; }
        public string ContractStartDate { get; set; }
        public string ContractEndDate { get; set; }

        public List<ContractPaymentUpdateDTO> Payments { get; set; } = new();
    }

    public class ContractPaymentUpdateDTO
    {
        public int Id { get; set; } // 0 => new
        public decimal Amount { get; set; }
        public string Status { get; set; }  // PaymentStatus enum (string)
        public string PaymentDate { get; set; }
    }


}

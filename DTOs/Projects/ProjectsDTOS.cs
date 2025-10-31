namespace ERP.DTOs.Projects
{
    public class ProjectCreateDto
    {
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public int ClientId { get; set; }
        public int? BrokerId { get; set; }
        public int QuotationId { get; set; }
        public string StartDate { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
    }

    public class ProjectUpdateDto
    {
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public int? BrokerId { get; set; }
        public ProjectStatus Status { get; set; }
    }

    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string ClientName { get; set; }
        public string BrokerName { get; set; }
        public string Status { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalProfit { get; set; }
    }

    public class ProjectPaymentDto
    {
        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
        public PaymentType Type { get; set; }
        public string Method { get; set; }
        public string Notes { get; set; }
    }

    public class ProjectTaskDto
    {
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public int? AssignedToId { get; set; }
    }

    public class ProjectAttachmentDto
    {
        public int ProjectId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string UploadedBy { get; set; }
    }
}

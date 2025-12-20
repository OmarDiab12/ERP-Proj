namespace ERP.DTOs.Invoices
{
    public class InvoiceDueReminderDTO
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string? DueDate { get; set; }
        public decimal Total { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Remaining => Total - PaidAmount;
        public int DaysUntilDue { get; set; }
        public bool IsOverdue { get; set; }
        public string? NextDueDate { get; set; }
        public decimal? NextDueAmount { get; set; }
        public bool NextDueIsOverdue { get; set; }
        public string DueSource { get; set; }
        public int? SupplierId { get; set; }
        public int? ClientId { get; set; }
        public int? ProjectId { get; set; }
        public string Status { get; set; }
    }
}

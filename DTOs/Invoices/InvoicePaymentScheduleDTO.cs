namespace ERP.DTOs.Invoices
{
    public class InvoicePaymentScheduleDTO
    {
        public int Id { get; set; }
        public string DueDate { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public string? Notes { get; set; }
    }
}

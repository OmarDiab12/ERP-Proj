namespace ERP.DTOs.Invoices
{
    public class InvoiceReminderFilterDTO
    {
        public int? DaysAhead { get; set; }
        public bool IncludeOverdue { get; set; } = true;
    }
}

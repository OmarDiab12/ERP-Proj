namespace ERP.DTOs.Invoices
{
    public class InvoiceFilterDTO
    {
        public string? Type { get; set; }
        public string? PaymentType { get; set; }
        public string? Status { get; set; }
        public int? SupplierId { get; set; }
        public int? ClientId { get; set; }
        public int? ProjectId { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }
}

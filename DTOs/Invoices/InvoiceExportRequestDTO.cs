namespace ERP.DTOs.Invoices
{
    public class InvoiceExportRequestDTO
    {
        public string Format { get; set; } = "excel";
        public InvoiceFilterDTO? Filter { get; set; }
        public InvoiceBrandingDTO? Branding { get; set; }
    }
}

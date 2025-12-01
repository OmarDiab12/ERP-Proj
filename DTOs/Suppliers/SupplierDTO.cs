namespace ERP.DTOs.Suppliers
{
    public class SupplierDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal CurrentBalance { get; set; }
    }

    public class SupplierWithInvoicesDTO
    {
        public SupplierDTO Supplier { get; set; }
        public List<Invoices.InvoiceDTO> Invoices { get; set; } = new();
        public decimal OutstandingBalance { get; set; }
    }
}

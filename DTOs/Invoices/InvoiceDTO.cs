using ERP.Helpers;

namespace ERP.DTOs.Invoices
{
    public class InvoiceAttachmentDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public class InvoiceItemDTO
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }

    public class InvoiceDTO
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string? DueDate { get; set; }
        public Enums.InvoiceType Type { get; set; }
        public Enums.InvoiceStatus Status { get; set; }
        public Enums.InvoicePaymentType PaymentType { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public decimal PaidAmount { get; set; }
        public string? Currency { get; set; }
        public string? Notes { get; set; }
        public int? SupplierId { get; set; }
        public int? ClientId { get; set; }
        public int? ProjectId { get; set; }
        public List<InvoiceItemDTO> Items { get; set; } = new();
        public List<InvoiceAttachmentDTO> Attachments { get; set; } = new();
    }
}

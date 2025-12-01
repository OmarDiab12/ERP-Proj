using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ERP.DTOs.Invoices
{
    public class CreateInvoiceDTO
    {
        [Required]
        public string InvoiceNumber { get; set; }

        [Required]
        public string InvoiceDate { get; set; }
        public string? DueDate { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string PaymentType { get; set; }
        public string? PaymentMethod { get; set; }

        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal PaidAmount { get; set; }
        public string? Currency { get; set; }
        public string? Notes { get; set; }

        public int? SupplierId { get; set; }
        public int? ClientId { get; set; }
        public int? ProjectId { get; set; }

        [MinLength(1)]
        public List<InvoiceItemInputDTO> Items { get; set; } = new();

        public List<InvoicePaymentScheduleInputDTO> PaymentSchedules { get; set; } = new();

        public IFormFileCollection? Attachments { get; set; }
    }
}

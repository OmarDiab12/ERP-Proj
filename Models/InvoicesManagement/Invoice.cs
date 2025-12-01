using ERP.Helpers;
using ERP.Models.ClientsManagement;
using ERP.Models.Projects;
using ERP.Models.SuppliersManagement;
using System.ComponentModel.DataAnnotations;

namespace ERP.Models.InvoicesManagement
{
    public class Invoice : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime? DueDate { get; set; }

        public Enums.InvoiceType Type { get; set; }

        public Enums.InvoiceStatus Status { get; set; }

        public Enums.InvoicePaymentType PaymentType { get; set; }

        [MaxLength(100)]
        public string? PaymentMethod { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Discount { get; set; }

        public decimal Tax { get; set; }

        public decimal Total { get; set; }

        public decimal PaidAmount { get; set; }

        [MaxLength(5)]
        public string? Currency { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public int? ClientId { get; set; }
        public Client? Client { get; set; }

        public int? ProjectId { get; set; }
        public Project? Project { get; set; }

        public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public virtual ICollection<InvoiceAttachment> Attachments { get; set; } = new List<InvoiceAttachment>();
    }
}

using ERP.Models.InventoryManagement;
using System.ComponentModel.DataAnnotations;

namespace ERP.Models.InvoicesManagement
{
    public class InvoiceItem : BaseEntity
    {
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public int? ProductId { get; set; }
        public InventoryItem? Product { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Discount { get; set; }

        public decimal Total { get; set; }
    }
}

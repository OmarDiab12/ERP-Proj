using System.ComponentModel.DataAnnotations;

namespace ERP.Models.InventoryManagement
{
    public class InventoryItem : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? ImagePath { get; set; }

        [MaxLength(255)]
        public string? ImageName { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }

        public int Quantity { get; set; }

        public int LowStockThreshold { get; set; }

        public int? PreferredSupplierId { get; set; }
    }
}

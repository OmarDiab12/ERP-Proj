using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ERP.DTOs.Inventory
{
    public class CreateInventoryItemDTO
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public int LowStockThreshold { get; set; }
        public int? PreferredSupplierId { get; set; }
        public IFormFile? Image { get; set; }
    }
}

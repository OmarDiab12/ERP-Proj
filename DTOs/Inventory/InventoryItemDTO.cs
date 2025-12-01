namespace ERP.DTOs.Inventory
{
    public class InventoryItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public int LowStockThreshold { get; set; }
        public int? PreferredSupplierId { get; set; }
        public string? ImagePath { get; set; }
        public string? ImageName { get; set; }
    }
}

namespace ERP.DTOs.Inventory
{
    public class InventoryLowStockAlertDTO
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int LowStockThreshold { get; set; }
    }
}

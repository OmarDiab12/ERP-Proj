using ERP.Models.InventoryManagement;

namespace ERP.Repositories.Interfaces.Inventory
{
    public interface IInventoryRepository : IBaseRepository<InventoryItem>
    {
        Task<bool> AdjustStockAsync(int itemId, int delta, int updatedBy);
    }
}

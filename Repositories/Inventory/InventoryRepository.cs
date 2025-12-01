using ERP.Models.InventoryManagement;
using ERP.Repositories.Interfaces.Inventory;
using ERP.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.Inventory
{
    public class InventoryRepository : BaseRepository<InventoryItem>, IInventoryRepository
    {
        public InventoryRepository(ERPDBContext context) : base(context)
        {
        }

        public async Task<bool> AdjustStockAsync(int itemId, int delta, int updatedBy)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(i => i.Id == itemId && !i.IsDeleted);
            if (entity == null)
                return false;

            entity.Quantity += delta;
            if (entity.Quantity < 0)
                entity.Quantity = 0;

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = updatedBy;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

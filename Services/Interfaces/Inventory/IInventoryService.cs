using ERP.DTOs.Inventory;

namespace ERP.Services.Interfaces.Inventory
{
    public interface IInventoryService
    {
        Task<ResponseDTO> CreateAsync(CreateInventoryItemDTO dto, int userId);
        Task<ResponseDTO> UpdateAsync(UpdateInventoryItemDTO dto, int userId);
        Task<ResponseDTO> GetAsync(int id);
        Task<ResponseDTO> GetAllAsync();
        Task<ResponseDTO> GetLowStockAsync();
        Task<ResponseDTO> NotifyLowStockAsync();
    }
}

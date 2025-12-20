using ERP.DTOs.Inventory;
using ERP.Models.InventoryManagement;
using ERP.Repositories.Interfaces;
using ERP.Repositories.Interfaces.Inventory;
using ERP.Repositories.Interfaces.Suppliers;
using ERP.Helpers;
using ERP.Services.Interfaces.Inventory;
using ERP.Services.Interfaces.Notifications;

namespace ERP.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repo;
        private readonly ISupplierRepository _supplierRepo;
        private readonly IErrorRepository _errors;
        private readonly IFileStorageService _fileStorage;
        private readonly INotificationService _notifications;

        public InventoryService(IInventoryRepository repo, ISupplierRepository supplierRepo, IErrorRepository errors, IFileStorageService fileStorage, INotificationService notifications)
        {
            _repo = repo;
            _supplierRepo = supplierRepo;
            _errors = errors;
            _fileStorage = fileStorage;
            _notifications = notifications;
        }

        public async Task<ResponseDTO> CreateAsync(CreateInventoryItemDTO dto, int userId)
        {
            const string fn = nameof(CreateAsync);
            try
            {
                if (dto.PreferredSupplierId.HasValue)
                {
                    var supplier = await _supplierRepo.GetByIdAsync(dto.PreferredSupplierId.Value);
                    if (supplier == null)
                        return new ResponseDTO { IsValid = false, Message = "Supplier not found" };
                }

                var entity = new InventoryItem
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    PurchasePrice = dto.PurchasePrice,
                    SalePrice = dto.SalePrice,
                    Quantity = dto.Quantity,
                    LowStockThreshold = dto.LowStockThreshold,
                    PreferredSupplierId = dto.PreferredSupplierId
                };

                if (dto.Image != null)
                {
                    var saved = await _fileStorage.SaveFileAsync(dto.Image, "Inventory");
                    if (saved != null)
                    {
                        entity.ImageName = saved.FileName;
                        entity.ImagePath = saved.RelativePath;
                    }
                }

                await _repo.CreateAsync(entity, userId);
                await NotifyLowStockAsync(new List<InventoryItem> { entity });
                return new ResponseDTO { IsValid = true, Data = entity.Id, Message = "Inventory item created" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetAllAsync()
        {
            const string fn = nameof(GetAllAsync);
            try
            {
                var list = await _repo.GetAllAsync();
                var dtos = list.Select(i => new InventoryItemDTO
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    PurchasePrice = i.PurchasePrice,
                    SalePrice = i.SalePrice,
                    Quantity = i.Quantity,
                    LowStockThreshold = i.LowStockThreshold,
                    PreferredSupplierId = i.PreferredSupplierId,
                    ImageName = i.ImageName,
                    ImagePath = i.ImagePath
                }).ToList();

                return new ResponseDTO { IsValid = true, Data = dtos };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetAsync(int id)
        {
            const string fn = nameof(GetAsync);
            try
            {
                var item = await _repo.GetByIdAsync(id);
                if (item == null)
                    return new ResponseDTO { IsValid = false, Message = "Item not found" };

                var dto = new InventoryItemDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    PurchasePrice = item.PurchasePrice,
                    SalePrice = item.SalePrice,
                    Quantity = item.Quantity,
                    LowStockThreshold = item.LowStockThreshold,
                    PreferredSupplierId = item.PreferredSupplierId,
                    ImageName = item.ImageName,
                    ImagePath = item.ImagePath
                };

                return new ResponseDTO { IsValid = true, Data = dto };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetLowStockAsync()
        {
            const string fn = nameof(GetLowStockAsync);
            try
            {
                var dtos = await BuildLowStockDTOsAsync();

                return new ResponseDTO { IsValid = true, Data = dtos };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> UpdateAsync(UpdateInventoryItemDTO dto, int userId)
        {
            const string fn = nameof(UpdateAsync);
            try
            {
                var entity = await _repo.GetByIdAsync(dto.Id);
                if (entity == null)
                    return new ResponseDTO { IsValid = false, Message = "Item not found" };

                entity.Name = dto.Name;
                entity.Description = dto.Description;
                entity.PurchasePrice = dto.PurchasePrice;
                entity.SalePrice = dto.SalePrice;
                entity.Quantity = dto.Quantity;
                entity.LowStockThreshold = dto.LowStockThreshold;
                entity.PreferredSupplierId = dto.PreferredSupplierId;

                if (dto.RemoveImage && !string.IsNullOrWhiteSpace(entity.ImagePath))
                {
                    await _fileStorage.DeleteFileAsync(entity.ImagePath);
                    entity.ImageName = null;
                    entity.ImagePath = null;
                }

                if (dto.Image != null)
                {
                    if (!string.IsNullOrWhiteSpace(entity.ImagePath))
                        await _fileStorage.DeleteFileAsync(entity.ImagePath);

                    var saved = await _fileStorage.SaveFileAsync(dto.Image, "Inventory");
                    if (saved != null)
                    {
                        entity.ImageName = saved.FileName;
                        entity.ImagePath = saved.RelativePath;
                    }
                }

                await _repo.UpdateAsync(entity, userId);
                await NotifyLowStockAsync(new List<InventoryItem> { entity });
                return new ResponseDTO { IsValid = true, Message = "Inventory item updated" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> NotifyLowStockAsync()
        {
            const string fn = nameof(NotifyLowStockAsync);
            try
            {
                var dtos = await BuildLowStockDTOsAsync();
                var alerts = dtos.Select(ToLowStockAlert).ToList();

                if (alerts.Any())
                {
                    await _notifications.BroadcastLowStockAsync(alerts);
                    return new ResponseDTO { IsValid = true, Data = alerts, Message = "Low stock alerts broadcasted" };
                }

                return new ResponseDTO { IsValid = true, Data = alerts, Message = "No low stock items found" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        private static bool IsLowStock(InventoryItem item)
        {
            return item.LowStockThreshold > 0 && item.Quantity <= item.LowStockThreshold;
        }

        private static InventoryItemDTO ToDTO(InventoryItem i)
        {
            return new InventoryItemDTO
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                PurchasePrice = i.PurchasePrice,
                SalePrice = i.SalePrice,
                Quantity = i.Quantity,
                LowStockThreshold = i.LowStockThreshold,
                PreferredSupplierId = i.PreferredSupplierId,
                ImageName = i.ImageName,
                ImagePath = i.ImagePath
            };
        }

        private static InventoryLowStockAlertDTO ToLowStockAlert(InventoryItemDTO dto)
        {
            return new InventoryLowStockAlertDTO
            {
                ItemId = dto.Id,
                Name = dto.Name,
                Quantity = dto.Quantity,
                LowStockThreshold = dto.LowStockThreshold
            };
        }

        private async Task<List<InventoryItemDTO>> BuildLowStockDTOsAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Where(IsLowStock).Select(ToDTO).ToList();
        }

        private async Task NotifyLowStockAsync(List<InventoryItem> items)
        {
            var alerts = items.Where(IsLowStock)
                .Select(i => ToLowStockAlert(ToDTO(i)))
                .ToList();

            if (alerts.Any())
                await _notifications.BroadcastLowStockAsync(alerts);
        }
    }
}

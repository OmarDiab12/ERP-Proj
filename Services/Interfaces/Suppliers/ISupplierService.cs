using ERP.DTOs.Suppliers;

namespace ERP.Services.Interfaces.Suppliers
{
    public interface ISupplierService
    {
        Task<ResponseDTO> CreateAsync(CreateSupplierDTO dto, int userId);
        Task<ResponseDTO> UpdateAsync(UpdateSupplierDTO dto, int userId);
        Task<ResponseDTO> GetAsync(int id);
        Task<ResponseDTO> GetAllAsync();
    }
}

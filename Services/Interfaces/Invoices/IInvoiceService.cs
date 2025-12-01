using ERP.DTOs.Invoices;

namespace ERP.Services.Interfaces.Invoices
{
    public interface IInvoiceService
    {
        Task<ResponseDTO> CreateAsync(CreateInvoiceDTO dto, int userId);
        Task<ResponseDTO> UpdateAsync(UpdateInvoiceDTO dto, int userId);
        Task<ResponseDTO> GetAsync(int id);
        Task<ResponseDTO> GetAllAsync();
    }
}

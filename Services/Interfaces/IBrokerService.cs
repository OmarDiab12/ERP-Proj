using ERP.DTOs.Brokers;

namespace ERP.Services.Interfaces
{
    public interface IBrokerService
    {
        Task<ResponseDTO> CreateBroker(CreateBrokerDTO dTO, int createdBy);
        Task<ResponseDTO> EditBroker(EditBrokerDTO dTO, int updatedBy);
        Task<ResponseDTO> GetBrokerAsync(int brokerId);
        Task<ResponseDTO> GetAllBrokersAsync();

    }
}

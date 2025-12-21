using ERP.DTOs.Clients;

namespace ERP.Services.Interfaces.Persons
{
    public interface IClientService
    {
        Task<ResponseDTO> CreateClient(CreateClientDTO dto, int createdBy);
        Task<ResponseDTO> EditClient(EditClientDTO dto, int updatedBy);
        Task<ResponseDTO> GetClientAsync(int clientId);
        Task<ResponseDTO> GetAllClientsAsync();
        Task<ResponseDTO> DeleteClient(int clientId, int createdBy);
    }
}

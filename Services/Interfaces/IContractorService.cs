using ERP.DTOs.Contractors;

namespace ERP.Services.Interfaces
{
    public interface IContractorService
    {
        Task<ResponseDTO> CreateContractor(CreateContratorDTO dTO, int createdBy);
        Task<ResponseDTO> EditContractor(EditContractorDTO dTO, int updatedBy);
        Task<ResponseDTO> GetContractorAsync(int contractorId);
        Task<ResponseDTO> GetAllContractorsAsync();
    }
}

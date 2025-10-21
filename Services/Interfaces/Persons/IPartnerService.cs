using ERP.DTOs;
using ERP.DTOs.Partners;

namespace ERP.Services.Interfaces.Persons
{
    public interface IPartnerService
    {
        Task<ResponseDTO> GetAllPartners();
        Task<ResponseDTO> CreateNewPartner(CreatePartnerDTO dTO);
        Task<ResponseDTO> EditPartner(EditPartnerDTO dTO);
        Task<ResponseDTO> GetPartner(int partnerId);
    }
}

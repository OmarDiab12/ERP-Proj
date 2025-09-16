using ERP.DTOs.Partners;
using ERP.Models.Partners;

namespace ERP.Repositories.Interfaces
{
    public interface IPartnerRepository : IBaseRepository<Partner>
    {
        Task<List<PartnerTransaction>> GetTransactionsForPartner(int partnerId);
        Task<bool> CreateNewPartner(List<PartnersShareDTO> partnersShares, Partner newPartner);
        Task<bool> EditPartnerAsync(int partnerIdToEdit, EditPartnerDTO updateData);
    }
}

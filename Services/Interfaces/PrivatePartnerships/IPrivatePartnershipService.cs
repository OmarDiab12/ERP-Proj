using ERP.DTOs;
using ERP.DTOs.PrivatePartnerships;

namespace ERP.Services.Interfaces.PrivatePartnerships
{
    public interface IPrivatePartnershipService
    {
        Task<ResponseDTO> CreateProjectAsync(CreatePrivatePartnershipProjectDTO dto, int userId);
        Task<ResponseDTO> AddTransactionAsync(int projectId, CreatePrivatePartnershipTransactionDTO dto, int userId);
        Task<ResponseDTO> GetProjectSummaryAsync(int projectId);
        Task<ResponseDTO> GetProjectsAsync();
    }
}

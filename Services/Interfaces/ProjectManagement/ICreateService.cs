using ERP.DTOs.Projects;
using System.Threading.Tasks;

namespace ERP.Services.Interfaces.ProjectManagement
{
    public interface ICreateService
    {
        Task<ResponseDTO> CreateFromQuotationAsync(int quotationId, int userId);
        Task<ResponseDTO> CreateFullProjectAsync(ProjectCreateFullDTO dto, int userId);
    }
}

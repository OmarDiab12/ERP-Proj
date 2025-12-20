using ERP.Models.EngineeringOffice;

namespace ERP.Repositories.Interfaces.EngineeringOffice
{
    public interface IEngineeringProjectRepository : IBaseRepository<EngineeringProject>
    {
        Task<EngineeringProject?> GetByIdWithAttachmentsAsync(int id);
        Task<IEnumerable<EngineeringProject>> GetAllWithAttachmentsAsync();
    }
}

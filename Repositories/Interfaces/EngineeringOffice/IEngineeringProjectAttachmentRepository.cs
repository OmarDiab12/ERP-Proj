using ERP.Models.EngineeringOffice;

namespace ERP.Repositories.Interfaces.EngineeringOffice
{
    public interface IEngineeringProjectAttachmentRepository : IBaseRepository<EngineeringProjectAttachment>
    {
        Task<IEnumerable<EngineeringProjectAttachment>> GetByProjectIdAsync(int projectId);
    }
}

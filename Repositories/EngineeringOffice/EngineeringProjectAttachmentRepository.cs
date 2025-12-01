using ERP.Models;
using ERP.Models.EngineeringOffice;
using ERP.Repositories.Interfaces.EngineeringOffice;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.EngineeringOffice
{
    public class EngineeringProjectAttachmentRepository : BaseRepository<EngineeringProjectAttachment>, IEngineeringProjectAttachmentRepository
    {
        public EngineeringProjectAttachmentRepository(ERPDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EngineeringProjectAttachment>> GetByProjectIdAsync(int projectId)
        {
            return await _dbSet
                .Where(a => a.EngineeringProjectId == projectId && !a.IsDeleted)
                .ToListAsync();
        }
    }
}

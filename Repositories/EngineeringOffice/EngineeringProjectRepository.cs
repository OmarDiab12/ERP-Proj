using ERP.Models;
using ERP.Models.EngineeringOffice;
using ERP.Repositories.Interfaces.EngineeringOffice;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.EngineeringOffice
{
    public class EngineeringProjectRepository : BaseRepository<EngineeringProject>, IEngineeringProjectRepository
    {
        public EngineeringProjectRepository(ERPDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EngineeringProject>> GetAllWithAttachmentsAsync()
        {
            return await _dbSet
                .Include(p => p.Attachments)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<EngineeringProject?> GetByIdWithAttachmentsAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Attachments)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }
    }
}

using ERP.Models;
using ERP.Models.PrivatePartnerships;
using ERP.Repositories.Interfaces.PrivatePartnerships;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.PrivatePartnerships
{
    public class PrivatePartnershipProjectRepository : BaseRepository<PrivatePartnershipProject>, IPrivatePartnershipProjectRepository
    {
        public PrivatePartnershipProjectRepository(ERPDBContext context) : base(context)
        {
        }

        public async Task<PrivatePartnershipProject> CreateProjectWithSharesAsync(PrivatePartnershipProject project, IEnumerable<PrivatePartnershipPartnerShare> shares, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            project.CreatedBy = userId;
            project.UpdatedBy = userId;
            project.CreatedAt = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.PrivatePartnershipProjects.AddAsync(project);
            await _context.SaveChangesAsync();

            var now = DateTime.UtcNow;
            foreach (var share in shares)
            {
                share.ProjectId = project.Id;
                share.CreatedBy = userId;
                share.UpdatedBy = userId;
                share.CreatedAt = now;
                share.UpdatedAt = now;
            }

            await _context.PrivatePartnershipPartnerShares.AddRangeAsync(shares);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return project;
        }

        public async Task<IEnumerable<PrivatePartnershipProject>> GetAllWithDetailsAsync()
        {
            return await _context.PrivatePartnershipProjects
                .Include(p => p.PartnerShares)
                .Include(p => p.Transactions)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<PrivatePartnershipProject?> GetProjectWithDetailsAsync(int projectId)
        {
            return await _context.PrivatePartnershipProjects
                .Include(p => p.PartnerShares)
                .Include(p => p.Transactions)
                .ThenInclude(t => t.PartnerShare)
                .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);
        }
    }
}

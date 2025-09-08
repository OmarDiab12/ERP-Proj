using ERP.Models;
using ERP.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly ERPDBContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ERPDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<T> CreateAsync(T entity, int createdBy)
        {
            entity.CreatedBy = createdBy;
            entity.UpdatedBy = createdBy;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> UpdateAsync(T entity, int updatedBy)
        {
            var existing = await _dbSet.FirstOrDefaultAsync(e => e.Id == entity.Id && !e.IsDeleted);
            if (existing == null)
                return false;

            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id, int updatedBy)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            if (entity == null)
                return false;

            entity.IsDeleted = true;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

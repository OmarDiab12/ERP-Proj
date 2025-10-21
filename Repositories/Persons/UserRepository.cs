using ERP.Models;
using ERP.Repositories.Interfaces.Persons;
using Microsoft.EntityFrameworkCore;

namespace ERP.Repositories.Persons
{
    public class UserRepository : BaseRepository<User> , IUserRepository
    {
        public UserRepository(ERPDBContext context) : base(context){}

        public Task<User?> GetByEmailAsync(string email) =>
            _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

        public Task<User?> GetByUserNameAsync(string userName) =>
            _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.FullName == userName);

        public Task<bool> EmailExistsAsync(string email) =>
            _context.Users.AnyAsync(u => u.Email == email);

        public Task<bool> UserNameExistsAsync(string userName) =>
            _context.Users.AnyAsync(u => u.FullName == userName);

        public async Task<IEnumerable<User>> GetPagedAsync(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            return await _context.Users.AsNoTracking()
                        .OrderByDescending(u => u.CreatedAt)
                        .Skip(skip).Take(pageSize).ToListAsync();
        }
    }
}

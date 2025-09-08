using Microsoft.EntityFrameworkCore;

namespace ERP.Models
{
    public class ERPDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Error> Errors { get; set; }

        public ERPDBContext() : base() { }
        public ERPDBContext(DbContextOptions<ERPDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var admin = new User
            {
                Id = 1,
                FullName = "System Admin",
                DisplayName = "Admin",
                Email = "admin@erp.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = 0,
                UpdatedBy = 0
            };

            modelBuilder.Entity<User>().HasData(admin);
        }
    }
}

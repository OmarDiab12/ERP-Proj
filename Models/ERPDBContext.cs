using ERP.Models.Brokers;
using ERP.Models.ClientsManagement;
using ERP.Models.ContractorsManagement;
using ERP.Models.Employees;
using ERP.Models.Partners;
using ERP.Models.Projects;
using ERP.Models.ProjectsManagement;
using ERP.Models.QoutationManagement;
using Microsoft.EntityFrameworkCore;

namespace ERP.Models
{
    public class ERPDBContext : DbContext
    {
        #region Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<PartnerTransaction> PartnerTransactions { get; set; }
        public DbSet<Broker> Brokers { get; set; }
        public DbSet<BrokerComission> brokerComissions { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Contractor> Contractors { get; set; }
        public DbSet<ContractOfContractor> ContractOfContracts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientAccountStatement> ClientStatements { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeTransaction> EmployeesTransactions { get; set; }
        public DbSet<OperationalExpense> OperationalExpenses { get; set; }
        public DbSet<PersonalLoan> PersonalLoans { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationItem> QuotationItems { get; set; }
        public DbSet<ProjectAttachment> ProjectAttachments { get; set; }
        public DbSet<ProjectExpense> ProjectExpenses { get; set; }
        public DbSet<ProjectPayment>  ProjectPayments { get; set; }
        public DbSet<ProjectProfitShare> ProjectProfitShares { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }

        #endregion


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

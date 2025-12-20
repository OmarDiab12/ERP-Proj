using Microsoft.EntityFrameworkCore;

namespace ERP.Models.Employees
{
    public class Employee : BaseEntity
    {
        public string Name { get; set; }
        public string? JobTitle { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        [Precision(18, 2)]
        public decimal? BaseSalary { get; set; }
        public virtual ICollection<EmployeeTransaction> EmployeeTransactions { get; set; } = new List<EmployeeTransaction>();
    }
}

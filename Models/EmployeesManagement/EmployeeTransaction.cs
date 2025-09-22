using ERP.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.Employees
{
    public class EmployeeTransaction : BaseEntity
    {
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }

        public TransactionType TransactionType { get; set; }
        [Precision(18, 2)]
        public decimal Amount { get; set; }
        
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}

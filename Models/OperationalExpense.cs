using ERP.Models.Projects;
using Microsoft.EntityFrameworkCore;

namespace ERP.Models
{
    public class OperationalExpense : BaseEntity
    {
        public string Description { get; set; }
        [Precision(18, 2)]
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Category { get; set; }

    }
}

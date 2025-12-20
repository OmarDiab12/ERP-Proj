using ERP.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.ProjectsManagement
{
    public class ProjectExpense : BaseEntity
    {
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public string Description { get; set; }
        [Precision(18, 2)]
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;

        public string Category { get; set; }
    }
}

using ERP.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.ProjectsManagement
{
    public class ProjectPayment : BaseEntity
    {
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Precision(18, 2)]
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentType Type { get; set; }   
        public string Method { get; set; }      
        public string Notes { get; set; }
    }
}

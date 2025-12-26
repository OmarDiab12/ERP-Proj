using ERP.Models.Employees;
using ERP.Models.Partners;
using ERP.Models.Projects;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.ProjectsManagement
{
    public class ProjectTask : BaseEntity
    {
        [ForeignKey(nameof(Project))]
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public TaskPriority Priority { get; set; }
        public ERP.Helpers.Enums.TaskStatus Status { get; set; }
        public TaskType TaskType { get; set; }
        public TaskReferenceType? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }

        [ForeignKey(nameof(AssignedTo))]
        public int? AssignedToId { get; set; }
        public virtual Employee? AssignedTo { get; set; }

        [ForeignKey(nameof(AssignedPartner))]
        public int? AssignedPartnerId { get; set; }
        public virtual Partner? AssignedPartner { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
    }
}

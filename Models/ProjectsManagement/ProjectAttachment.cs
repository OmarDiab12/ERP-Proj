using ERP.Models.Projects;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.ProjectsManagement
{
    public class ProjectAttachment : BaseEntity
    {
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }
        
    }
}

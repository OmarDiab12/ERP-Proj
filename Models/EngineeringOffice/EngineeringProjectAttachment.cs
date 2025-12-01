using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.EngineeringOffice
{
    public class EngineeringProjectAttachment : BaseEntity
    {
        [ForeignKey(nameof(Project))]
        public int EngineeringProjectId { get; set; }
        public virtual EngineeringProject Project { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }
}

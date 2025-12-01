using System.ComponentModel.DataAnnotations;

namespace ERP.Models.EngineeringOffice
{
    public class EngineeringProject : BaseEntity
    {
        [Required]
        public string ProjectName { get; set; } = string.Empty;

        [Required]
        public string ClientName { get; set; } = string.Empty;

        public string? ClientPhone { get; set; }
        public string? ClientEmail { get; set; }
        public string? ClientAddress { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<EngineeringProjectAttachment> Attachments { get; set; } = new List<EngineeringProjectAttachment>();
    }
}

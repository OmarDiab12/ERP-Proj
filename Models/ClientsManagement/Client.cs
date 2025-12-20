using ERP.Models.Projects;
using ERP.Models.QoutationManagement;
using System.ComponentModel.DataAnnotations;

namespace ERP.Models.ClientsManagement
{
    public class Client :BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public string? ImageUrl { get; set; } 
        public string? OriginalImageName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<ClientAccountStatement> AccountStatements { get; set; } = new List<ClientAccountStatement>();
        public virtual ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
    }
}

using System.ComponentModel.DataAnnotations;

namespace ERP.Models.ContractorsManagement
{
    public class Contractor : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public virtual ICollection<ContractOfContractor> Contracts { get; set; } = new List<ContractOfContractor>();
    }
}

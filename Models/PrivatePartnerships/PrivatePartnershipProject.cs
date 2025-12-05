using System.ComponentModel.DataAnnotations;

namespace ERP.Models.PrivatePartnerships
{
    public class PrivatePartnershipProject : BaseEntity
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<PrivatePartnershipPartnerShare> PartnerShares { get; set; } = new List<PrivatePartnershipPartnerShare>();

        public virtual ICollection<PrivatePartnershipTransaction> Transactions { get; set; } = new List<PrivatePartnershipTransaction>();
    }
}

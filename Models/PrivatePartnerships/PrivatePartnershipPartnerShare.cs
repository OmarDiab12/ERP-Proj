using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ERP.Models.PrivatePartnerships
{
    public class PrivatePartnershipPartnerShare : BaseEntity
    {
        [Required]
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }

        public virtual PrivatePartnershipProject Project { get; set; }

        [Required]
        [MaxLength(250)]
        public string PartnerName { get; set; } = string.Empty;

        [Precision(5, 2)]
        public double ContributionPercentage { get; set; }

        [Precision(18, 2)]
        public decimal ContributionAmount { get; set; }

        public virtual ICollection<PrivatePartnershipTransaction> Transactions { get; set; } = new List<PrivatePartnershipTransaction>();
    }
}

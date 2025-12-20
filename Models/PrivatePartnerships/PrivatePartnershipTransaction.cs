using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ERP.Models.PrivatePartnerships
{
    public class PrivatePartnershipTransaction : BaseEntity
    {
        [Precision(18, 2)]
        public decimal Amount { get; set; }

        [Required]
        public PrivatePartnershipTransactionType TransactionType { get; set; }

        [MaxLength(500)]
        public string Note { get; set; } = string.Empty;

        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }

        public virtual PrivatePartnershipProject Project { get; set; }

        [ForeignKey(nameof(PartnerShare))]
        public int? PartnerShareId { get; set; }

        public virtual PrivatePartnershipPartnerShare? PartnerShare { get; set; }

        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}

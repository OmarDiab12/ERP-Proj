using ERP.Models.Partners;
using ERP.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.ProjectsManagement
{
    public class ProjectProfitShare : BaseEntity
    {
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        [ForeignKey(nameof(Partner))]
        public int PartnerId { get; set; }
        public virtual Partner Partner { get; set; }
        [Precision(18, 2)]
        public decimal PartnerPercentage { get; set; }
        [Precision(18, 2)]
        public decimal DistributedAmount { get; set; }
        public DateTime DistributionDate { get; set; } = DateTime.UtcNow;
    }
}

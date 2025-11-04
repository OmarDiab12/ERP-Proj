using ERP.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.ContractorsManagement
{
    public class ContractOfContractor : BaseEntity
    {
        [ForeignKey(nameof(Contractor))]
        public int ContractorId { get; set; }
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        [Precision(18, 2)]
        public decimal ContractAmount { get; set; }
        [Precision(18, 2)]
        public decimal TotalWithdrawals { get; set; }
        [Precision(18, 2)]
        public decimal RemainingBalance => ContractAmount - TotalWithdrawals;
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // A list of file paths or URLs for contract documents.
        public string ContractDocuments { get; set; }

        public virtual Contractor Contractor { get; set; }
        public virtual Project Project { get; set; }
    }
}

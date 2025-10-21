using ERP.Models.Brokers;
using ERP.Models.ClientsManagement;
using ERP.Models.ContractorsManagement;
using ERP.Models.ProjectsManagement;
using ERP.Models.QoutationManagement;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.Projects
{
    public class Project : BaseEntity
    {
        // Existing relationships
        public virtual ICollection<BrokerComission> BrokerComissions { get; set; } = new List<BrokerComission>();
        public virtual ICollection<ContractOfContractor> ContractOfContracts { get; set; } = new List<ContractOfContractor>();
        public virtual ICollection<ClientAccountStatement> ClientAccountStatements { get; set; } = new List<ClientAccountStatement>();

        [ForeignKey(nameof(Quotation))]
        public int quotationId { get; set; }
        public virtual Quotation Quotation { get; set; }

        // ===== New Recommended Fields =====

        [ForeignKey(nameof(Client))]
        public int? ClientId { get; set; }
        public virtual Client Client { get; set; }

        [ForeignKey(nameof(Broker))]
        public int? BrokerId { get; set; }
        public virtual Broker Broker { get; set; }

        public string ProjectName { get; set; }
        public string Description { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.InProgress;

        // Financials
        public decimal TotalCost { get; set; }
        public decimal TotalPaymentsReceived { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalContractorPayments { get; set; }

        [NotMapped]
        public decimal NetProfit => TotalPaymentsReceived - (TotalExpenses + TotalContractorPayments);

        // Navigation
        public virtual ICollection<ProjectExpense> ProjectExpenses { get; set; } = new List<ProjectExpense>();
        public virtual ICollection<ProjectPayment> ProjectPayments { get; set; } = new List<ProjectPayment>();
        public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
        public virtual ICollection<ProjectAttachment> ProjectAttachments { get; set; } = new List<ProjectAttachment>();
        public virtual ICollection<ProjectProfitShare> ProjectProfitShares { get; set; } = new List<ProjectProfitShare>();
    }
}

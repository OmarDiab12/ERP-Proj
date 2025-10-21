using ERP.Models.ClientsManagement;
using ERP.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.QoutationManagement
{
    public class Quotation : BaseEntity
    {
        // Foreign key to link this quote to a specific client.
        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }

        // Date the quotation was created.
        public DateTime QuotationDate { get; set; }

        // A status to track the quote's state (e.g., "Draft," "Sent," "Accepted," "Rejected").
        public QuotationStatus Status { get; set; }

        // The total price of the quote after all discounts.
        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }

        // Optional notes for the entire quotation.
        public string GeneralNotes { get; set; }

        // Navigation property to hold all the individual items in this quote.
        public virtual ICollection<QuotationItem> QuotationItems { get; set; } = new List<QuotationItem>();
        [ForeignKey(nameof(Project))]
        public int projectId { get; set; }
        public virtual Project Project { get; set; }
    }
}

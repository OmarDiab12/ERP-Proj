using System.ComponentModel.DataAnnotations;

namespace ERP.Models.InvoicesManagement
{
    public class InvoicePaymentSchedule : BaseEntity
    {
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public DateTime DueDate { get; set; }

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}

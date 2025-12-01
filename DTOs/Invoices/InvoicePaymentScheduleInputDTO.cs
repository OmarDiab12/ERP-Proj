using System.ComponentModel.DataAnnotations;

namespace ERP.DTOs.Invoices
{
    public class InvoicePaymentScheduleInputDTO
    {
        [Required]
        public string DueDate { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public bool IsPaid { get; set; }
        public string? Notes { get; set; }
    }
}

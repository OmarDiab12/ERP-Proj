using System.ComponentModel.DataAnnotations;
using ERP.Models.InvoicesManagement;

namespace ERP.Models.SuppliersManagement
{
    public class Supplier : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string? PhoneNumber { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public decimal OpeningBalance { get; set; }

        public decimal CurrentBalance { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}

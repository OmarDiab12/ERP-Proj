using System.ComponentModel.DataAnnotations;

namespace ERP.DTOs.Suppliers
{
    public class CreateSupplierDTO
    {
        [Required]
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public decimal OpeningBalance { get; set; }
    }
}

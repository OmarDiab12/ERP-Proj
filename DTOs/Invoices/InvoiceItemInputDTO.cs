using System.ComponentModel.DataAnnotations;

namespace ERP.DTOs.Invoices
{
    public class InvoiceItemInputDTO
    {
        public int? ProductId { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Discount { get; set; }
    }
}

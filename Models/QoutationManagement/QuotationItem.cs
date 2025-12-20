using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.QoutationManagement
{
    public class QuotationItem : BaseEntity
    {
        // Foreign key to link this item to a specific quotation.
        [ForeignKey(nameof(Quotation))]
        public int QuotationId { get; set; }
        public virtual Quotation Quotation { get; set; }

        // Description of the item (e.g., "Carpentry Work," "Plastering Work").
        public string Description { get; set; }

        // The original price of this item before any discounts.
        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

        // The quantity of the item (e.g., number of units, square meters, etc.).
        [Precision(18, 2)]
        public decimal Quantity { get; set; }

        // The discount applied to this specific item.
        [Range(0,1)]
        [Precision(5, 2)]
        public decimal DiscountPercentage { get; set; } = 0;

        // The final price of the item after the discount.
        public decimal FinalPrice => (UnitPrice * Quantity) * (1 - (DiscountPercentage / 100));

        // Specific notes for this item.
        public string ItemNotes { get; set; }
    }
}

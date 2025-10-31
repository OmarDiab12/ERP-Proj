using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.QoutationManagement
{
    public class QuotationAttachement : BaseEntity
    {
        [ForeignKey(nameof(Quotation))]
        public int QuotationId { get; set; }
        public virtual Quotation Quotation { get; set; }

        public string filePath { get; set; }
        public string fileName { get; set; }
    }
}


using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.Partners
{
    public class PartnerTransaction : BaseEntity
    {
        [Precision(18, 2)]
        public decimal Amount { get; set; } = 0;
        public partnerTransaction transationType {  get; set; }

        [ForeignKey(nameof(Partner))]
        public int PartnerId {  get; set; }
        public virtual Partner Partner { get; set; }
    }
}

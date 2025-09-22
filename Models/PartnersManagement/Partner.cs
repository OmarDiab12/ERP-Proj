using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ERP.Models.Partners
{
    public class Partner : BaseEntity
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        [Precision(5, 2)]
        public double ProjectShare { get; set; }
        public string AssignedTasks { get; set; } = string.Empty;
        public virtual ICollection<PartnerTransaction> transactions { get; set; } = new List<PartnerTransaction>();
    }
}

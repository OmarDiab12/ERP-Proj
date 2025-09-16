using ERP.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.Brokers
{
    public class BrokerComission : BaseEntity
    {
        [Precision(18, 2)]
        public decimal AmountRecieved { get; set; }
        [Precision(5, 2)]
        public decimal PercentofTotal {get; set; }

        public DateTime PayoutDate { get; set; }
        
        [Required]
        [ForeignKey(nameof(Broker))]
        public int BrokerId { get; set; }

        [Required]
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; } 

        public virtual Broker Broker { get; set; }
        public virtual Project Project { get; set; }
    }
}

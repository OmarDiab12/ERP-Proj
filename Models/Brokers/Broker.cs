using System.ComponentModel.DataAnnotations;

namespace ERP.Models.Brokers
{
    public class Broker : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } 

        [MaxLength(20),Phone]
        public string? PhoneNumber { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; } 
        public virtual ICollection<BrokerComission> Comissions { get; set; } = new List<BrokerComission>();
    }
}

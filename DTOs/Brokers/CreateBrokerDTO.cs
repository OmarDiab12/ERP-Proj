using System.ComponentModel.DataAnnotations;

namespace ERP.DTOs.Brokers
{
    public class CreateBrokerDTO
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(20), Phone]
        public string? PhoneNumber { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }
    }
}

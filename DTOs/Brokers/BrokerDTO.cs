using System.ComponentModel.DataAnnotations;

namespace ERP.DTOs.Brokers
{
    public class BrokerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public List<BrokerTransactionDTO> Transactions { get; set; } = new();
    }

    public class BrokerTransactionDTO
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public decimal PercentOfTotal { get; set; }
        public decimal AmountReceived { get; set; }
    }

}

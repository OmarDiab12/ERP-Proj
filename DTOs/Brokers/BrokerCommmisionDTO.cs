namespace ERP.DTOs.Brokers
{
    public class BrokerCommmisionDTO
    {
        public int ProjectId { get; set; }
        public int BrokerId { get; set; }
        public decimal CommissionPercentage { get; set; }
    }
}

namespace ERP.DTOs.PrivatePartnerships
{
    public class CreatePrivatePartnershipTransactionDTO
    {
        public decimal Amount { get; set; }
        public PrivatePartnershipTransactionType TransactionType { get; set; }
        public string Note { get; set; } = string.Empty;
        public string? OccurredAt { get; set; }
        public int? PartnerShareId { get; set; }
    }
}

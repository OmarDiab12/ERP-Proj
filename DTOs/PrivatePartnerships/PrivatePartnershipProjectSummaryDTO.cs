namespace ERP.DTOs.PrivatePartnerships
{
    public class PrivatePartnershipProjectSummaryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal TotalContribution { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalPartnerWithdrawals { get; set; }
        public decimal NetProfitOrLoss { get; set; }
        public List<PrivatePartnerShareSummaryDTO> Partners { get; set; } = new();
        public List<PrivatePartnershipTransactionViewDTO> Transactions { get; set; } = new();
    }

    public class PrivatePartnerShareSummaryDTO
    {
        public int Id { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public double ContributionPercentage { get; set; }
        public decimal ContributionAmount { get; set; }
        public decimal TotalWithdrawals { get; set; }
    }

    public class PrivatePartnershipTransactionViewDTO
    {
        public int Id { get; set; }
        public PrivatePartnershipTransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
        public string PartnerName { get; set; } = string.Empty;
    }
}

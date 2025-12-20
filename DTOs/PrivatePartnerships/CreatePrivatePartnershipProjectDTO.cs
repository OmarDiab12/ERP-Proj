namespace ERP.DTOs.PrivatePartnerships
{
    public class CreatePrivatePartnershipProjectDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<PrivatePartnerShareDTO> PartnerShares { get; set; } = new();
    }

    public class PrivatePartnerShareDTO
    {
        public string PartnerName { get; set; } = string.Empty;
        public double ContributionPercentage { get; set; }
        public decimal ContributionAmount { get; set; }
    }
}

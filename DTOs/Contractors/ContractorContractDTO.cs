namespace ERP.DTOs.Contractors
{
    public class ContractorContractDTO
    {
        public int ProjectId { get; set; }
        public int ContractorId { get; set; }
        public decimal ContractAmount { get; set; }
        public string WorkDescription { get; set; }
    }
}

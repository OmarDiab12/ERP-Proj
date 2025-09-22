using ERP.Models.ContractorsManagement;
using ERP.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.DTOs.Contractors
{
    public class ContractorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<ContractofContractorDTO> contractofContractorDTOs { get; set; } = new List<ContractofContractorDTO>();
    }

    public class ContractofContractorDTO
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public decimal ContractAmount { get; set; }
        public decimal TotalWithdrawals { get; set; }
        public decimal RemainingBalance => ContractAmount - TotalWithdrawals;
        public List<string> ContractDocuments { get; set; } = new List<string>();

    }
}

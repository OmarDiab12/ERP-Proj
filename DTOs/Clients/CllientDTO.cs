using ERP.Models.ClientsManagement;
using ERP.Models.Projects;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.DTOs.Clients
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        public string? ImageUrl { get; set; }          // رابط كامل
        public string? OriginalImageName { get; set; } // الاسم الأصلي

        public List<ClientStatementDTO> ClientStatements { get; set; } = new();
    }

    public class ClientStatementDTO
    {
        public int? ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}

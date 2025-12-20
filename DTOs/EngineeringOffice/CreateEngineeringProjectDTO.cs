using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ERP.DTOs.EngineeringOffice
{
    public class CreateEngineeringProjectDTO
    {
        [Required]
        public string ProjectName { get; set; }

        [Required]
        public string ClientName { get; set; }

        public string? ClientPhone { get; set; }
        public string? ClientEmail { get; set; }
        public string? ClientAddress { get; set; }
        public string? Description { get; set; }

        public IFormFileCollection? Attachments { get; set; }
    }
}

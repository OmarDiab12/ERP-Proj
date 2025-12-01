namespace ERP.DTOs.EngineeringOffice
{
    public class EngineeringProjectDTO
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public string? ClientPhone { get; set; }
        public string? ClientEmail { get; set; }
        public string? ClientAddress { get; set; }
        public string? Description { get; set; }
        public List<EngineeringProjectAttachmentDTO> Attachments { get; set; } = new();
    }
}

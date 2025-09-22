namespace ERP.DTOs.Clients
{
    public class CreateClientDTO
    {
        public string Name { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}

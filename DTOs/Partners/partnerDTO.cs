namespace ERP.DTOs.Partners
{
    public class partnerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string AssignedTasks { get; set; } = string.Empty;
        public double Share { get; set; } = 0;
        public double Withdrawls { get; set; } = 0; 
        public double profits { get; set; } = 0;
        public double capitals { get; set; } = 0;
    }
}

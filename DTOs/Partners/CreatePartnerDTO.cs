namespace ERP.DTOs.Partners
{
    public class CreatePartnerDTO
    {
        public string Name { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string AssignedTasks { get; set; } = string.Empty;
        public double share { get; set; } = 0;
        public List<PartnersShareDTO> partnersShareDTOs { get; set; }= new List<PartnersShareDTO>();
    }

    public class PartnersShareDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double share { get; set; } = 0;
    }
}

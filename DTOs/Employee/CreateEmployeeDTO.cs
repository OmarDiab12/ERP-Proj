namespace ERP.DTOs.Employee
{
    public class CreateEmployeeDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public decimal? BaseSalary { get; set; }
    }
}

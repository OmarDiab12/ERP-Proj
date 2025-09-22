namespace ERP.DTOs.Employee
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public decimal? BaseSalary { get; set; }
        public decimal Balance { get; set; } // مجموع المعاملات (+/-)
        public List<EmployeeTransactionDTO> Transactions { get; set; } = new();
    }
}

namespace ERP.DTOs.Employee
{
    public class CreateEmployeeTransactionDTO
    {
        public string TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public int EmployeeId { get; set; }
    }

    public class EditEmployeeTransactionDTO : CreateEmployeeTransactionDTO
    {
        public int Id { get; set; }
    }

    public class EmployeeTransactionDTO
    {
        public int Id { get; set; }
        public string TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public int EmployeeId { get; set; }

    }
}

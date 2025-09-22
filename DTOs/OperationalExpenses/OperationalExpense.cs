namespace ERP.DTOs.OperationalExpenses
{
    public class CreateOperationalExpenseDTO
    {
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string ExpenseDate { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class EditOperationalExpenseDTO : CreateOperationalExpenseDTO
    {
        public int Id { get; set; }
    }

    public class OperationalExpenseDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string ExpenseDate { get; set; } = string.Empty; 
        public string Category { get; set; } = string.Empty;
    }

    public class ExpenseRangeRequestDTO
    {
        public string DateFrom { get; set; } = string.Empty; 
        public string DateTo { get; set; } = string.Empty; 
        public string? Category { get; set; } 
    }
}

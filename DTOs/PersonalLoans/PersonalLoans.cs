namespace ERP.DTOs.PersonalLoans
{
    public class CreatePersonalLoanDTO
    {
        public string PersonName { get; set; } = string.Empty;
        public string IssueDate { get; set; } = string.Empty; // yyyy-MM-dd
        public decimal Amount { get; set; }
        public string RepaymentDate { get; set; } = string.Empty; // yyyy-MM-dd
    }

    public class EditPersonalLoanDTO : CreatePersonalLoanDTO
    {
        public int Id { get; set; }
        public bool IsRepaid { get; set; }
    }

    public class PersonalLoanDTO
    {
        public int Id { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public string IssueDate { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string RepaymentDate { get; set; } = string.Empty;
        public bool IsRepaid { get; set; }
        public string? LastReminderSent { get; set; }
    }

}

using Microsoft.EntityFrameworkCore;

namespace ERP.Models
{
    public class PersonalLoan : BaseEntity
    {
        public string PersonName { get; set; }
        public DateTime IssueDate { get; set; }
        [Precision(18, 2)]
        public decimal Amount { get; set; }
        public DateTime RepaymentDate { get; set; }
        public bool IsRepaid { get; set; } = false;
        public DateTime? LastReminderSent { get; set; }
    }
}

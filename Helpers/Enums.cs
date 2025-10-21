namespace ERP.Helpers
{
    public class Enums
    {
        public enum partnerTransaction
        {
            CapitalAddition,
            Withdrawl
        }

        public enum TransactionType
        {
            Salary, 
            Advance, 
            Deduction,
            Bonus
        }

        public enum QuotationStatus
        {
            Draft,
            Sent,
            Accepted,
            Rejected
        }

        public enum ProjectStatus
        {
            PendingApproval = 0,
            InProgress = 1,
            Completed = 2,
            OnHold = 3,
            Canceled = 4
        }

        public enum PaymentType
        {
            Income = 1,
            Expense = 2
        }

        public enum TaskPriority 
        { 
            Low, 
            Medium, 
            High, 
            Urgent 
        }
        
        public enum TaskStatus 
        { 
            New, 
            InProgress, 
            Completed, 
            Delayed 
        }
    }
}

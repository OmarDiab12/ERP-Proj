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

        public enum PaymentStatus
        {
            Paid = 1,
            Pending = 2
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

        public enum InvoiceType
        {
            Purchase = 1,
            Sale = 2
        }

        public enum InvoiceStatus
        {
            Draft = 0,
            Unpaid = 1,
            PartiallyPaid = 2,
            Paid = 3,
            Overdue = 4
        }

        public enum InvoicePaymentType
        {
            Cash = 1,
            Credit = 2
        }
    }
}

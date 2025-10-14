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
    }
}

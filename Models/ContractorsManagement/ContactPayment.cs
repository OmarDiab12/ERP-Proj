using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.ContractorsManagement
{
    public class ContactPayment : BaseEntity
    {
        [ForeignKey(nameof(ContractOfContractor))]
        public int ContractId { get; set; }
        public virtual ContractOfContractor Contract { get; set; }
        public int index { get; set; }
        public decimal amount { get; set; }
        public PaymentStatus status { get; set; }
        public DateTime dateTime { get; set; }

    }
}

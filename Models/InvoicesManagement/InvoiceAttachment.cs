namespace ERP.Models.InvoicesManagement
{
    public class InvoiceAttachment : BaseEntity
    {
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}

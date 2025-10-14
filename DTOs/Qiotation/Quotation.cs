namespace ERP.DTOs.Qiotation
{
    public class CreateQuotationItemDTO
    {
        public string Description { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        /// Discount can be either fraction (0..1) or percent (0..100).
        public decimal DiscountPercentage { get; set; } = 0m;
        public string? ItemNotes { get; set; }
    }

    public class CreateQuotationDTO
    {
        public int ClientId { get; set; }
        public string QuotationDate { get; set; } = string.Empty; // "yyyy-MM-dd"
        public string Status { get; set; } = "Draft";
        public string? GeneralNotes { get; set; }
        public List<CreateQuotationItemDTO> Items { get; set; } = new();
    }

    public class EditQuotationItemDTO : CreateQuotationItemDTO
    {
        public int? Id { get; set; } // optional: supplied when editing existing items
    }

    public class EditQuotationDTO : CreateQuotationDTO
    {
        public int Id { get; set; }
        public List<EditQuotationItemDTO> Items { get; set; } = new();
    }

    public class QuotationItemDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal FinalPrice { get; set; }
        public string? ItemNotes { get; set; }
    }

    public class QuotationBasicDTO
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string QuotationDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }

    public class QuotationDTO : QuotationBasicDTO
    {
        public string? GeneralNotes { get; set; }
        public List<QuotationItemDTO> Items { get; set; } = new();
    }
}

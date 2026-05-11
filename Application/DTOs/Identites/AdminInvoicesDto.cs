namespace Application.DTOs.Identites
{
    public class AdminInvoiceDto
    {
        public string BranchCode { get; set; } = string.Empty;
        public string BoothName { get; set; } = string.Empty;
        public int BoothId { get; set; }
        public string InvoiceCode { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? VoucherCode { get; set; }
        public float? DiscountPercent { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
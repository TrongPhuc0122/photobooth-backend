namespace Application.DTOs.Identites
{
    public class InvoiceDto
    {
        public string BoothName { get; set; } = string.Empty;
        public string InvoiceCode { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? VoucherCode { get; set; }
        public float? DiscountPercent { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
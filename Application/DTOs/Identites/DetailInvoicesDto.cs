using Shared;

namespace Application.DTOs.Identites
{
    public class DetailInvoiceDto
    {
        public Guid BoothId { get; set; }
        public BasicInfor Infor { get; set; } = new();
        public PaymentDetail Payment { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
public class BasicInfor
{
    public int InvoiceId { get; set; }
    public string InvoiceCode { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string BoothName { get; set; } = string.Empty;
}
public class PaymentDetail
{
    public decimal Price { get; set; }
    public string? VoucherCOde { get; set; }
    public float? DiscountPercent { get; set; }
    public decimal FinalPrice { get; set; }
    public PaymentMethodStatus PaymentMethod { get; set; }
}
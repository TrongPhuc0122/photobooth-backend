namespace Application.DTOs.Identites;

public class InvoiceDto
{
    public InvoiceBasicInfor Infor { get; set; } = new();
    public string? VoucherCode { get; set; } = string.Empty;
    public string InvoiceCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
public class InvoiceBasicInfor
{
    public decimal Price { get; set; }
    public float DiscountPercent { get; set; }
    public decimal FinalPrice { get; set; }
}
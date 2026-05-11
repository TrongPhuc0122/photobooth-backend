namespace Application.DTOs.Identites;

public class VoucherDto
{
    public string VoucherCode {get; set; } = string.Empty;
    public string Purpose {get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public float DiscountPercent { get; set; }
    public int UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public bool BeUsed { get; set; }
}
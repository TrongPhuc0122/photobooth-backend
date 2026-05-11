namespace Application.DTOs.Identites;

public class CreateVoucherDto
{
    public required string VoucherCode { get; set; }
    public required float DiscountPercent { get; set; }
    public required string Purpose { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required int UsageLimit { get; set; }
    public required string Creater { get; set; }
}
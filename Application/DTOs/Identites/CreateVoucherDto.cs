namespace Application.DTOs.Identites;

public class CreateVoucherDto
{
    public string? BranchCode { get; set; }
    public required string VoucherCode { get; set; }
    public required float DiscountPercent { get; set; }
    public required string Purpose { get; set; }
    public required DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? UsageLimit { get; set; }
    public required string Creator { get; set; }
}
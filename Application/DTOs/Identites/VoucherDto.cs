using Shared;

namespace Application.DTOs.Identites;

public class VoucherDto
{
    public VoucherBasicInfor Infor { get; set; } = new();
    public int? BranchId { get; set; }
    public string Purpose {get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public UsedStatus BeUsed { get; set; }
}
public class VoucherBasicInfor
{
    public int VoucherId { get; set; }
    public string VoucherCode {get; set; } = string.Empty;
    public float DiscountPercent { get; set; }
}
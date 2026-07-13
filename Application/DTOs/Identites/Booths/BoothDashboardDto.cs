using Shared;

namespace Application.DTOs.Identites.Booths;

public class BoothDashboardDto
{
    public Guid BoothId { get; set; }
    public string BoothName { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public Status Status { get; set; }
    public string BoothIp { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
}
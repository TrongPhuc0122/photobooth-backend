using Shared;
namespace Application.DTOs.Identites.Booths;

public class BoothDto
{
    public BoothBasicInfor Infor { get; set; }= new();
    public string BranchName { get; set; } = string.Empty;
    public string BoothIp { get; set; } = string.Empty;
    public string? Creator { get; set; }
    public string Brand { get; set; } = string.Empty;
    public Status Status { get; set; }
    public int PaperCount { get; set; }
    public int PaperMax { get; set; }
    public int RibbonCount { get; set; }
    public int RibbonMax { get; set; }
    public decimal MonthlyRevenue { get; set; }
}
public class BoothBasicInfor
{
    public Guid BoothId { get; set; }
    public string BoothName { get; set; } = string.Empty; 
}
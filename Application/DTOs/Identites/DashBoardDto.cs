using Application.DTOs.Identites.Booths;
using Shared;

namespace Application.DTOs.Identites;
public class DashBoradDto
{
    public RevenueInfor Revenue { get; set; } = new();
    public IEnumerable<BoothInfor> BoothStatuses { get; set; } = [];
}
public class RevenueInfor
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public decimal FinalPrice { get; set; }
    public IEnumerable<RevenueDataPoint> DataPoints { get; set; } = [];
}

public class RevenueDataPoint
{
    public DateTime Date { get; set; }
    public decimal FinalPrice { get; set; }
}
public class BoothInfor
{
    public Guid BoothId { get; set; }
    public string BoothName { get ;set; } = string.Empty;
    public Status status { get; set; }
    public int PaperCount { get; set; }
    public int RibbonCount { get; set; }
}
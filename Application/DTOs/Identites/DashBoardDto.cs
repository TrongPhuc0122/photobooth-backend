using Shared;
public class DashBoardDto
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TransactionCount { get; set; }
    public TodayRevenueDto? Today { get; set; }
    public List<DailyRevenuePoint>? Last7Days { get; set; }
    public MonthlyRevenueDto? Last6Months { get; set; }
    public MonthDetailDto? MonthDetail { get; set; }
    public List<BoothRevenueDto> ByBooth { get; set; } = new();
    public List<BoothRevenueDto> TopBooths { get; set; } = new();
    public List<PaymentMethodRevenueDto> ByPaymentMethod { get; set; } = new();
    public List<BoothInfor> BoothIssues { get; set; } = new();
}
public class TodayRevenueDto
{
    public decimal FinalPrice { get; set; }
    public int TransactionCount { get; set; }
    public decimal YesterdayFinalPrice { get; set; }
    public float ComparePercent { get; set; }
}
public class DailyRevenuePoint
{
    public DateTime Date { get; set; }
    public decimal FinalPrice { get; set; }
    public int TransactionCount { get; set; }
}
public class MonthlyRevenueDto
{
    public List<MonthlyRevenuePoint> DataPoints { get; set; } = new();
    public decimal Total6Months { get; set; }        
    public decimal AveragePerMonth { get; set; }     
    public decimal CurrentMonthRevenue { get; set; }  
    public float CompareWithLastMonth { get; set; }  
}
public class MonthlyRevenuePoint
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal FinalPrice { get; set; }
}
public class MonthDetailDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal FinalPrice { get; set; }
    public int TransactionCount { get; set; }
    public decimal AveragePerDay { get; set; }        
}
public class BoothRevenueDto
{
    public Guid BoothId { get; set; }
    public string BoothName { get; set; } = string.Empty;
    public decimal FinalPrice { get; set; }
    public int TransactionCount { get; set; }
    public float Percent { get; set; }
}
public class PaymentMethodRevenueDto
{
    public PaymentMethodStatus PaymentMethod { get; set; }
    public decimal FinalPrice { get; set; }
    public int TransactionCount { get; set; }
    public float Percent { get; set; }
}
public class BoothInfor
{
    public Guid BoothId { get; set; }
    public string BoothName { get ;set; } = string.Empty;
    public Status status { get; set; }
    public int PaperCount { get; set; }
    public int RibbonCount { get; set; }
}
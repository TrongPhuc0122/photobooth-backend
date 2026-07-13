using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;
using Application.DTOs.Identites;
using Application.DTOs.Identites.Booths;
using Application.Interfaces;
using Application.Interfaces.Commons;
using Domain.Entities;
using Shared;
using Shared.Results;

namespace Application.Services;

public class DashBoardService : IDashBoardService
{
    private readonly IGenericRepository<Booths, Guid> _boothRepository;
    private readonly IGenericRepository<Invoice, int> _invoiceRepository;
    private readonly IGenericRepository<Branch, int> _branchRepository;

    public DashBoardService(
        IGenericRepository<Booths, Guid> boothRepository,
        IGenericRepository<Invoice, int> invoiceRepository,
        IGenericRepository<Branch, int> branchRepository)
    {
        _boothRepository = boothRepository;
        _invoiceRepository = invoiceRepository;
        _branchRepository = branchRepository;
    }

    // API
    public async Task<ServiceResult<DashBoardDto>> Get7DaysDashboard()
    {
        DateTime now = DateTime.UtcNow;
        var From = now.Date.AddDays(-7);
        var invoices = await LoadInvoices(From, now);
        var booths = await LoadBooths();
        
        var dto = await ProcessData(invoices, booths, From, now);
        dto.Last7Days = Enumerable.Range(0,7)
                        .Select(offset => From.Date.AddDays(offset))
                        .Select(date => new DailyRevenuePoint
                        {
                            Date = date,
                            FinalPrice = invoices.Where(i => i.CreatedAt.Date == date).Sum(i => i.FinalPrice),
                            TransactionCount = invoices.Count()
                        })
                        .ToList();
        return ServiceResult<DashBoardDto>.Success(dto);
    }
    public async Task<ServiceResult<DashBoardDto>> Get6MonthsDashboard()
    {
        DateTime now = DateTime.UtcNow;
        var From = now.Date.AddMonths(-6);
        var invoices = await LoadInvoices(From, now);
        var booths = await LoadBooths();

        var totalRevenue = invoices.Sum(i => i.FinalPrice);
        var dataPoints = Enumerable.Range(0,6)
                        .Select(offset => From.AddMonths(offset ))
                        .Select(month => new MonthlyRevenuePoint
                        {
                            Month = month.Month,
                            Year = month.Year,
                            FinalPrice = invoices
                                            .Where(i => i.CreatedAt.Year == month.Year 
                                                    && i.CreatedAt.Month == month.Month)
                                            .Sum(i => i.FinalPrice)
                        })
                        .ToList();
        var lastMonthRevenue = invoices
                                    .Where(i => i.CreatedAt >= new DateTime(now.Year, now.Month, 1).AddMonths(-1)
                                            && i.CreatedAt < new DateTime(now.Year, now.Month, 1))
                                    .Sum(i => i.FinalPrice);
        var thisMonthRevenue = invoices
                                    .Where(i => i.CreatedAt >= new DateTime(now.Year, now.Month, 1)
                                            && i.CreatedAt <= now)
                                    .Sum(i => i.FinalPrice);

        var dto = await ProcessData(invoices, booths, From, now);
        dto.Last6Months = new MonthlyRevenueDto
        {
            DataPoints = dataPoints,
            Total6Months = totalRevenue,
            AveragePerMonth = totalRevenue / 6m,
            CurrentMonthRevenue = thisMonthRevenue,
            CompareWithLastMonth = lastMonthRevenue > 0 
                                    ? (thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue * 100m
                                    : 0m
        };
        return ServiceResult<DashBoardDto>.Success(dto);
    }
    public async Task<ServiceResult<DashBoardDto>> GetDashboard(DateTime From, DateTime To)
    {
        var invoices = await LoadInvoices(From, To);
        var booths = await LoadBooths();

        var totalRevenue = invoices.Sum(i => i.FinalPrice);

        var totalDays = (To - From).TotalDays;
        string granularity = totalDays <= 31 ? "Day"
                            : totalDays <= 730 ? "Month" 
                            : "Year";
        List<CustomRevenuePoint> dataPoints;

        switch (granularity)
        {
            case "Day":
                dataPoints = invoices
                                .GroupBy(i => i.CreatedAt.Date)
                                .OrderBy(g => g.Key)
                                .Select(g => new CustomRevenuePoint
                                {
                                    Label = g.Key.ToString("yyyy-MM-dd"),
                                    FinalPrice = g.Sum(i => i.FinalPrice),
                                    TransactionCount = g.Count()
                                })        
                                .ToList();
                break;
            case "Month":
                dataPoints = invoices
                                .GroupBy(i => new { i.CreatedAt.Year, i.CreatedAt.Month })
                                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                                .Select(g => new CustomRevenuePoint
                                {
                                    Label = $"{g.Key.Month:D2}/{g.Key.Year}",
                                    FinalPrice = g.Sum(i => i.FinalPrice),
                                    TransactionCount = g.Count()
                                })        
                                .ToList();
                break;
            default:
                dataPoints = invoices      
                                .GroupBy(i => i.CreatedAt.Year)
                                .OrderBy(g => g.Key)
                                .Select(g => new CustomRevenuePoint
                                {
                                    Label = g.Key.ToString(),
                                    FinalPrice = g.Sum(i => i.FinalPrice),
                                    TransactionCount = g.Count()
                                })
                                .ToList();
                break;
        }
        var dto = await ProcessData(invoices, booths, From, To);
        dto.Custom = new CustomRevenueDto
        {
            Granularity = granularity,
            DataPoints = dataPoints,
            TotalRevenue = totalRevenue,
            TransactionCount = invoices.Count(),
            AveragePerPeriod = dataPoints.Count > 0 ? totalRevenue / dataPoints.Count : 0m
        };
        return ServiceResult<DashBoardDto>.Success(dto);
    }

    // Load data
    private Task<List<InvoiceDashboardDto>> LoadInvoices(DateTime From, DateTime To)
    {
        var result = _invoiceRepository.Query()
        .Where(i => i.CreatedAt >= From && i.CreatedAt <= To && i.FlowStatus)
        .Select(i => new InvoiceDashboardDto
        {
            FinalPrice = i.FinalPrice,
            CreatedAt = i.CreatedAt,
            PaymentMethod = i.PaymentMethod,
            BoothId = i.BoothId
        })
        .ToList();
        return Task.FromResult(result);
    }
    private Task<List<BoothDashboardDto>> LoadBooths()
    {
        var result = _boothRepository.Query()
                        .Select(b => new BoothDashboardDto
                        {
                            BoothId = b.BoothId,
                            BoothName = b.BoothName,
                            BranchId = b.BranchId,
                            Status = b.BoothHealth != null ? b.BoothHealth.Status : Status.OFFLINE,
                            BoothIp = b.BoothIp
                        })
                        .OrderBy(b => b.Status == Status.ERROR ? 1
                                    : b.Status == Status.OFFLINE ? 2
                                    : b.Status == Status.ONLINE ? 3
                                    : 4)
                        .ToList();
        return Task.FromResult(result);
    }

    // Process data
    private async Task<TodayRevenueDto> GetTodaySummary()
    {
        DateTime now = DateTime.UtcNow;
        var invoices = await LoadInvoices(now.Date, now);
        var yesterday = await LoadInvoices(now.Date.AddDays(-1), now.Date);

        var TodayRevenue = invoices.Sum(i => i.FinalPrice);
        var YesterdayRevenue = yesterday.Sum(i => i.FinalPrice);
        var result = new TodayRevenueDto
        {
            FinalPrice = TodayRevenue,
            TransactionCount = invoices.Count(),
            YesterdayFinalPrice = YesterdayRevenue,
            ComparePercent = YesterdayRevenue == 0 ? 100 
                            : (float)(TodayRevenue - YesterdayRevenue) / (float)(YesterdayRevenue) * 100.0f
        };
        return result;
    }
    private async Task<MonthDetailDto> GetMonthSummary()
    {
        DateTime now = DateTime.UtcNow;

        var thisMonth = await LoadInvoices(new DateTime(now.Year, now.Month, 1), now);
        var revenue = thisMonth.Sum(i => i.FinalPrice);
        var result = new MonthDetailDto
        {
            Month = now.Month,
            Year = now.Year,
            FinalPrice = revenue,
            TransactionCount = thisMonth.Count(),
            AveragePerDay = revenue / DateTime.DaysInMonth(now.Year, now.Month)
        };
        return result;
    }
    private async Task<DashBoardDto> ProcessData(List<InvoiceDashboardDto> invoices, List<BoothDashboardDto> booths, DateTime From, DateTime To)
    {
        var revenue = invoices.Sum(i => i.FinalPrice);
        var byBooth = invoices
                        .GroupBy(b => b.BoothId)
                        .Select(g => new BoothRevenueDto
                        {
                            BoothId = g.Key,
                            BoothName = booths.FirstOrDefault(b => b.BoothId == g.Key)?.BoothName ?? string.Empty,
                            FinalPrice = g.Sum(i => i.FinalPrice),
                            TransactionCount = g.Count(),
                            Percent = revenue > 0 
                                        ? (float)(g.Sum(i => i.FinalPrice) / revenue) * 100.0f 
                                        : 0f
                        })
                        .OrderByDescending(b => b.FinalPrice)
                        .ToList();
        
        var result = new DashBoardDto
        {
            From = From,
            To = To,
            Today = await GetTodaySummary(),
            MonthDetail = await GetMonthSummary(),
            ByBooth = byBooth,
            TopBooths = byBooth.Take(5).ToList(),
            ByPaymentMethod = invoices
                                .GroupBy(i => i.PaymentMethod)
                                .Select(g => new PaymentMethodRevenueDto
                                {
                                    PaymentMethod = g.Key,
                                    FinalPrice = g.Sum(i => i.FinalPrice),
                                    TransactionCount = g.Count(),
                                    Percent = revenue > 0 
                                        ? (float)(g.Sum(i => i.FinalPrice) / revenue) * 100.0f 
                                        : 0f
                                })
                                .OrderByDescending(i => i.FinalPrice)
                                .ToList(),
            BoothIssues = booths
                            .Select(g => new BoothInfor
                            {
                                BoothId = g.BoothId,
                                BoothName = g.BoothName ?? string.Empty,
                                status = g.Status
                            })
                            .OrderBy(b => b.status)
                            .ToList(),
            BranchCount = _branchRepository.Query().Count()
        };
        return result;
    }
}
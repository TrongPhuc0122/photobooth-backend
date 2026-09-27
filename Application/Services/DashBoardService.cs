using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Application.DTOs.Identites;
using Application.DTOs.Identites.Booths;
using Application.Interfaces;
using Application.Interfaces.Commons;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
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
    #region Today Summary
    public async Task<ServiceResult<TodayRevenueDto>> GetTodaySummary()
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
            ComparePercent = TodayRevenue == 0 ?  
                            0.0f
                            : (float)(TodayRevenue - YesterdayRevenue) / (float)(TodayRevenue) * 100.0f
        };
        return ServiceResult<TodayRevenueDto>.Success(result);
    }
    #endregion

    #region  Line Chart 
    public async Task<ServiceResult<List<DailyRevenuePoint>>> Get7DaysDashboard()
    {
        DateTime now = DateTime.UtcNow;
        var invoices = await LoadInvoices(now.AddDays(-7), now);
        var result = GroupRevenue(
            invoices,
            i => i.CreatedAt.Date,
            g => new DailyRevenuePoint
            {
                Date = g.Key,
                FinalPrice = g.Sum(i =>i.FinalPrice),
                TransactionCount = g.Count()
            }
        );
        return ServiceResult<List<DailyRevenuePoint>>.Success(result);
    }

    public async Task<ServiceResult<MonthlyRevenueDto>> Get6MonthsDashboard()
    {
        DateTime now = DateTime.UtcNow;
        var invoices = await LoadInvoices(now.AddMonths(-6), now);
        var monthlyResult = GroupRevenue(
            invoices,
            i => new DateTime(i.CreatedAt.Year, i.CreatedAt.Month, 1),
            g => new MonthlyRevenuePoint
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                FinalPrice = g.Sum(i => i.FinalPrice)
            }
        );
        decimal totalRevenue = invoices.Sum(i => i.FinalPrice);
        
        DateTime startOfThisMonth = new DateTime(now.Year, now.Month, 1);
        DateTime startOfLastMonth = startOfThisMonth.AddMonths(-1);

        decimal totalRevenueLastMonth = invoices
            .Where(i => i.CreatedAt >= startOfLastMonth && i.CreatedAt < startOfThisMonth)
            .Sum(i => i.FinalPrice);
            
        decimal totalRevenueThisMonth = invoices
            .Where(i => i.CreatedAt.Month == now.Month && i.CreatedAt.Year == now.Year)
            .Sum(i => i.FinalPrice);
        
        var result = new MonthlyRevenueDto
        {
            DataPoints = monthlyResult,
            Total6Months = totalRevenue,
            AveragePerMonth = totalRevenue/(decimal)6,
            CompareWithLastMonth = totalRevenueLastMonth == 0 ?
                                    (decimal)0
                                    : (totalRevenueThisMonth - totalRevenueLastMonth)/totalRevenueLastMonth * (decimal)100
        };
        return ServiceResult<MonthlyRevenueDto>.Success(result);
    }

    public async Task<ServiceResult<CustomRevenueDto>> GetDashboard(DateTime From, DateTime To)
    {
        var invoices = await LoadInvoices(From, To);
        var totalDays = (To - From).TotalDays;
        string granularity = totalDays <= 31 ? "Day"
                            : totalDays <= 730 ? "Month"
                            : "Year";

        decimal totalRevenue = invoices.Sum(i => i.FinalPrice);
        int transactionCount = invoices.Count();

        switch (granularity)
        {
            case "Day":
                {
                    var routineResult = GroupRevenue(
                        invoices,
                        i => i.CreatedAt.Date,
                        g => new CustomRevenuePoint
                        {
                            Label = g.Key.ToString("yyyy-MM-dd"),
                            FinalPrice = g.Sum(i => i.FinalPrice)
                        }
                    );
                    var result = new CustomRevenueDto
                    {
                        Granularity = granularity,
                        DataPoints = routineResult,
                        TotalRevenue = totalRevenue,
                        TransactionCount = transactionCount,
                        AveragePerPeriod = totalRevenue / (decimal)totalDays
                    };
                    return ServiceResult<CustomRevenueDto>.Success(result);
                }
            case "Month":
                {
                    int totalMonths = (To.Year - From.Year) * 12 + (To.Month - From.Month);
                    var monthlyResult = GroupRevenue(
                        invoices,
                        i => new DateTime(i.CreatedAt.Year, i.CreatedAt.Month, 1),
                        g => new CustomRevenuePoint
                        {
                            Label = $"{g.Key.Month:D2}/{g.Key.Year}",
                            FinalPrice = g.Sum(i => i.FinalPrice)
                        }
                    );
                    var result = new CustomRevenueDto
                    {
                        Granularity = granularity,
                        DataPoints = monthlyResult,
                        TotalRevenue = totalRevenue,
                        AveragePerPeriod = totalRevenue/(decimal)totalMonths
                    };
                    return ServiceResult<CustomRevenueDto>.Success(result);
                }
            case "Year":
                {
                    var yearlyResult = GroupRevenue(
                        invoices,
                        i => i.CreatedAt.Year,
                        g => new CustomRevenuePoint
                        {
                            Label = g.Key.ToString(),
                            FinalPrice = g.Sum(i => i.FinalPrice)
                        }
                    );
                    var result = new CustomRevenueDto
                    {
                        Granularity = granularity,
                        DataPoints = yearlyResult,
                        TotalRevenue = totalRevenue,
                        AveragePerPeriod = yearlyResult.Count > 0 ? totalRevenue / yearlyResult.Count : 0m
                    };
                    return ServiceResult<CustomRevenueDto>.Success(result);
                }
            default: break;
        }
        return ServiceResult<CustomRevenueDto>.InternalServerError("Fail to load Dashboard");
    }

    #endregion

    #region Bar Chart 
    public async Task<ServiceResult<List<BoothRevenueDto>>> GetTopBoothsIn7Days()
    {
        DateTime now = DateTime.UtcNow;
        var invoices = await LoadInvoices(now.AddDays(-7), now);
        var booths = await LoadBooths();
        var totalRevenue = invoices.Sum(i => i.FinalPrice);

        var byBooths = GroupRevenue(
            invoices,
            i => i.BoothId,
            g => new BoothRevenueDto
            {
                BoothId = g.Key,
                BoothName = booths.FirstOrDefault(b => b.BoothId == g.Key)?.BoothName ?? "",
                FinalPrice = g.Sum(i => i.FinalPrice),
                TransactionCount = g.Count(),
                Percent = totalRevenue > 0 ? (float)(g.Sum(i => i.FinalPrice) / totalRevenue) * 100f : 0f
            }
        ).ToList();
        var result = byBooths
            .OrderByDescending(b => b.FinalPrice)
            .Take(5)
            .ToList();
        return ServiceResult<List<BoothRevenueDto>>.Success(result);
    }

    public async Task<ServiceResult<List<BoothRevenueDto>>> GetTopBoothsIn6Months()
    {
        DateTime now = DateTime.UtcNow;
        var invoices = await LoadInvoices(now.AddMonths(-6), now);
        var booths = await LoadBooths();
        var totalRevenue = invoices.Sum(i => i.FinalPrice);

        var byBooths = GroupRevenue(
            invoices,
            i => i.BoothId,
            g => new BoothRevenueDto
            {
                BoothId = g.Key,
                BoothName = booths.FirstOrDefault(b => b.BoothId == g.Key)?.BoothName ?? "",
                FinalPrice = g.Sum(i => i.FinalPrice),
                TransactionCount = g.Count(),
                Percent = totalRevenue > 0 ? (float)(g.Sum(i => i.FinalPrice) / totalRevenue) * 100f : 0f
            }
        ).ToList();
        var result = byBooths
            .OrderByDescending(b => b.FinalPrice)
            .Take(5)
            .ToList();
        return ServiceResult<List<BoothRevenueDto>>.Success(result);
    }

    public async Task<ServiceResult<List<BoothRevenueDto>>> GetTopBoothsInCustom(DateTime From, DateTime To)
    {
        var totalDays = (To - From).TotalDays;
        var invoices = await LoadInvoices(From, To);
        var booths = await LoadBooths();
        var totalRevenue = invoices.Sum(i => i.FinalPrice);

        var byBooths = GroupRevenue(
            invoices,
            i => i.BoothId,
            g => new BoothRevenueDto
            {
                BoothId = g.Key,
                BoothName = booths.FirstOrDefault(b => b.BoothId == g.Key)?.BoothName ?? "",
                FinalPrice = g.Sum(i => i.FinalPrice),
                TransactionCount = g.Count(),
                Percent = totalRevenue > 0 ? (float)(g.Sum(i => i.FinalPrice) / totalRevenue) * 100f : 0f
            }
        ).ToList();
        var result = byBooths
            .OrderByDescending(b => b.FinalPrice)
            .Take(5)
            .ToList();
        return ServiceResult<List<BoothRevenueDto>>.Success(result);
        
    }
    #endregion

    #region  Pie Chart 
    public async Task<ServiceResult<List<PaymentMethodRevenueDto>>> GetPaymentMethodIn7Days()
    {
        DateTime now = DateTime.UtcNow;
        var invoices = await LoadInvoices(now.AddDays(-7), now);
        var totalRevenue = invoices.Sum(i => i.FinalPrice);
        var result = GroupRevenue(
            invoices,
            i => i.PaymentMethod,
            g => new PaymentMethodRevenueDto
            {
                PaymentMethod = g.Key,
                FinalPrice = g.Sum(i => i.FinalPrice),
                TransactionCount = g.Count(),
                Percent = totalRevenue > 0 ? (float)(g.Sum(i => i.FinalPrice) / totalRevenue) * 100f : 0f
            }
        );
        return ServiceResult<List<PaymentMethodRevenueDto>>.Success(result);
    }

    public async Task<ServiceResult<List<PaymentMethodRevenueDto>>> GetPaymentMethodIn6Months()
    {
        DateTime now = DateTime.UtcNow;
        var invoices = await LoadInvoices(now.AddMonths(-6), now);
        var totalRevenue = invoices.Sum(i => i.FinalPrice);
        var result = GroupRevenue(
            invoices,
            i => i.PaymentMethod,
            g => new PaymentMethodRevenueDto
            {
                PaymentMethod = g.Key,
                FinalPrice = g.Sum(i => i.FinalPrice),
                TransactionCount = g.Count(),
                Percent = totalRevenue > 0 ? (float)(g.Sum(i => i.FinalPrice) / totalRevenue) * 100f : 0f
            }
        );
        return ServiceResult<List<PaymentMethodRevenueDto>>.Success(result);
    }
    public async Task<ServiceResult<List<PaymentMethodRevenueDto>>> GetPaymentMethodInCustom(DateTime From, DateTime To)
    {
        var invoices = await LoadInvoices(From, To);
        var totalRevenue = invoices.Sum(i => i.FinalPrice);
        var result = GroupRevenue(
            invoices,
            i => i.PaymentMethod,
            g => new PaymentMethodRevenueDto
            {
                PaymentMethod = g.Key,
                FinalPrice = g.Sum(i => i.FinalPrice),
                TransactionCount = g.Count(),
                Percent = totalRevenue > 0 ? (float)(g.Sum(i => i.FinalPrice) / totalRevenue) * 100f : 0f
            }
        );
        return ServiceResult<List<PaymentMethodRevenueDto>>.Success(result);
    }
    #endregion
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

    private List<TResult> GroupRevenue<TKey, TResult>(
        List<InvoiceDashboardDto> invoices,
        Func<InvoiceDashboardDto, TKey> keySelector,
        Func<IGrouping<TKey, InvoiceDashboardDto>, TResult> resultSelector
    )
    {
        return invoices
            .GroupBy(keySelector)
            .Select(resultSelector)
            .ToList();
    }
}
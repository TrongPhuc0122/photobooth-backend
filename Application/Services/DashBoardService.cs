using Application.DTOs.Identites;
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

    public DashBoardService(
        IGenericRepository<Booths, Guid> boothRepository,
        IGenericRepository<Invoice, int> invoiceRepository)
    {
        _boothRepository = boothRepository;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<ServiceResult<DashBoardDto>> GetDashboard(DateTime? from, DateTime? to)
    {
        var now = DateTime.UtcNow;
        var isDefault = from == null || to == null;
        var effectiveFrom = from ?? new DateTime(now.Year, now.Month, 1);
        var effectiveTo = to ?? now;

        IEnumerable<Invoice> invoices;
        try
        {
            var queryFrom = isDefault
                ? new DateTime(effectiveTo.Year, effectiveTo.Month, 1).AddMonths(-5)
                : effectiveFrom;

            invoices = _invoiceRepository.GetMulti(
                i => i.CreatedAt >= queryFrom && i.CreatedAt <= effectiveTo && i.FlowStatus,
                includes: ["Booth"]
            );
        }
        catch (Exception ex)
        {
            return ServiceResult<DashBoardDto>.InternalServerError($"Lỗi truy vấn hóa đơn: {ex.Message}");
        }

        IEnumerable<Booths> boothIssues;
        try
        {
            boothIssues = _boothRepository.GetMulti(
                b => b.BoothErrors.Any(e => !e.IsFixed)
                    || (b.BoothResources != null && (
                        (b.BoothResources.PaperMax > 0 && (float)b.BoothResources.PaperCount / b.BoothResources.PaperMax <= 0.1f)
                        || (b.BoothResources.RibbonMax > 0 && (float)b.BoothResources.RibbonCount / b.BoothResources.RibbonMax <= 0.1f)
                    )),
                includes: ["BoothErrors", "BoothHealth", "BoothResources"]
            );
        }
        catch (Exception ex)
        {
            return ServiceResult<DashBoardDto>.InternalServerError($"Lỗi truy vấn booth: {ex.Message}");
        }

        try
        {
            var rangeInvoices = invoices
                .Where(i => i.CreatedAt >= effectiveFrom && i.CreatedAt <= effectiveTo)
                .ToList();

            var totalRevenue = rangeInvoices.Sum(i => i.FinalPrice);

            var byBooth = rangeInvoices
                .Where(i => i.Booth != null)
                .GroupBy(i => new { i.Booth!.BoothId, i.Booth.BoothName })
                .Select(g => new BoothRevenueDto
                {
                    BoothId = g.Key.BoothId,
                    BoothName = g.Key.BoothName,
                    FinalPrice = g.Sum(i => i.FinalPrice),
                    TransactionCount = g.Count(),
                    Percent = totalRevenue == 0 ? 0
                        : (float)(g.Sum(i => i.FinalPrice) / totalRevenue * 100)
                })
                .OrderByDescending(b => b.FinalPrice)
                .ToList();

            var byPayment = rangeInvoices
                .GroupBy(i => i.PaymentMethod)
                .Select(g => new PaymentMethodRevenueDto
                {
                    PaymentMethod = g.Key,
                    FinalPrice = g.Sum(i => i.FinalPrice),
                    TransactionCount = g.Count(),
                    Percent = totalRevenue == 0 ? 0
                        : (float)(g.Sum(i => i.FinalPrice) / totalRevenue * 100)
                })
                .OrderByDescending(p => p.FinalPrice)
                .ToList();

            var boothIssueDto = boothIssues
                .OrderBy(b => b.BoothErrors.Any(e => !e.IsFixed) ? 0 : 1)
                .Select(b => new BoothInfor
                {
                    BoothId = b.BoothId,
                    BoothName = b.BoothName,
                    status = b.BoothHealth?.Status ?? Status.OFFLINE,
                    PaperCount = b.BoothResources?.PaperCount ?? 0,
                    RibbonCount = b.BoothResources?.RibbonCount ?? 0
                }).ToList();

            var board = new DashBoardDto
            {
                From = effectiveFrom,
                To = effectiveTo,
                TotalRevenue = totalRevenue,
                TransactionCount = rangeInvoices.Count,
                ByBooth = byBooth,
                TopBooths = byBooth.Take(5).ToList(),
                ByPaymentMethod = byPayment,
                BoothIssues = boothIssueDto
            };

            if (isDefault)
            {
                var todayInvoices = invoices.Where(i => i.CreatedAt.Date == now.Date).ToList();
                var yesterdayInvoices = invoices.Where(i => i.CreatedAt.Date == now.Date.AddDays(-1)).ToList();
                var todayTotal = todayInvoices.Sum(i => i.FinalPrice);
                var yesterdayTotal = yesterdayInvoices.Sum(i => i.FinalPrice);

                board.Today = new TodayRevenueDto
                {
                    FinalPrice = todayTotal,
                    TransactionCount = todayInvoices.Count,
                    YesterdayFinalPrice = yesterdayTotal,
                    ComparePercent = yesterdayTotal == 0 ? 0
                        : (float)((todayTotal - yesterdayTotal) / yesterdayTotal * 100)
                };

                board.Last7Days = Enumerable.Range(0, 7)
                    .Select(offset =>
                    {
                        var date = now.Date.AddDays(-6 + offset);
                        var dayInvoices = invoices.Where(i => i.CreatedAt.Date == date).ToList();
                        return new DailyRevenuePoint
                        {
                            Date = date,
                            FinalPrice = dayInvoices.Sum(i => i.FinalPrice),
                            TransactionCount = dayInvoices.Count
                        };
                    }).ToList();

                var last6MonthsStart = new DateTime(now.Year, now.Month, 1).AddMonths(-5);
                var monthPoints = Enumerable.Range(0, 6)
                    .Select(offset =>
                    {
                        var month = last6MonthsStart.AddMonths(offset);
                        var monthInvoices = invoices
                            .Where(i => i.CreatedAt.Month == month.Month && i.CreatedAt.Year == month.Year)
                            .ToList();
                        return new MonthlyRevenuePoint
                        {
                            Month = month.Month,
                            Year = month.Year,
                            FinalPrice = monthInvoices.Sum(i => i.FinalPrice)
                        };
                    }).ToList();

                var currentMonthRevenue = monthPoints.Last().FinalPrice;
                var lastMonthRevenue = monthPoints[^2].FinalPrice;

                board.Last6Months = new MonthlyRevenueDto
                {
                    DataPoints = monthPoints,
                    Total6Months = monthPoints.Sum(m => m.FinalPrice),
                    AveragePerMonth = monthPoints.Average(m => m.FinalPrice),
                    CurrentMonthRevenue = currentMonthRevenue,
                    CompareWithLastMonth = lastMonthRevenue == 0 ? 0
                        : (float)((currentMonthRevenue - lastMonthRevenue) / lastMonthRevenue * 100)
                };

                var thisMonthInvoices = invoices
                    .Where(i => i.CreatedAt.Month == now.Month && i.CreatedAt.Year == now.Year)
                    .ToList();

                board.MonthDetail = new MonthDetailDto
                {
                    Month = now.Month,
                    Year = now.Year,
                    FinalPrice = thisMonthInvoices.Sum(i => i.FinalPrice),
                    TransactionCount = thisMonthInvoices.Count,
                    AveragePerDay = thisMonthInvoices.Sum(i => i.FinalPrice) / DateTime.DaysInMonth(now.Year, now.Month)
                };
            }

            return ServiceResult<DashBoardDto>.Success(board);
        }
        catch (Exception ex)
        {
            return ServiceResult<DashBoardDto>.InternalServerError($"Lỗi xử lý dashboard: {ex.Message}");
        }
    }   
}
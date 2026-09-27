using Application.DTOs.Identites;
using Application.DTOs.Identites.Booths;
using Shared.Results;

namespace Application.Interfaces;   

public interface IDashBoardService
{
    // line chart
    Task<ServiceResult<List<DailyRevenuePoint>>> Get7DaysDashboard();
    Task<ServiceResult<MonthlyRevenueDto>> Get6MonthsDashboard();
    Task<ServiceResult<CustomRevenueDto>> GetDashboard(DateTime From, DateTime To);

    // bar chart - booth
    Task<ServiceResult<List<BoothRevenueDto>>> GetTopBoothsIn7Days();
    Task<ServiceResult<List<BoothRevenueDto>>> GetTopBoothsIn6Months();
    Task<ServiceResult<List<BoothRevenueDto>>> GetTopBoothsInCustom(DateTime From, DateTime To);

    // Pie chart
    Task<ServiceResult<List<PaymentMethodRevenueDto>>> GetPaymentMethodIn7Days();
    Task<ServiceResult<List<PaymentMethodRevenueDto>>> GetPaymentMethodIn6Months();
    Task<ServiceResult<List<PaymentMethodRevenueDto>>> GetPaymentMethodInCustom(DateTime From, DateTime To);

    // today summary — tách riêng, không phụ thuộc range
    Task<ServiceResult<TodayRevenueDto>> GetTodaySummary();
}
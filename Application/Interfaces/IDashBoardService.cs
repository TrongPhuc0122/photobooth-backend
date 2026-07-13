using Application.DTOs.Identites;
using Application.DTOs.Identites.Booths;
using Shared.Results;

namespace Application.Interfaces;   

public interface IDashBoardService
{
    Task<ServiceResult<DashBoardDto>> GetDashboard(DateTime From, DateTime To);
    Task<ServiceResult<DashBoardDto>> Get7DaysDashboard();
    Task<ServiceResult<DashBoardDto>> Get6MonthsDashboard();
}
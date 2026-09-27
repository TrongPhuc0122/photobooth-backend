using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Results;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashBoardController : BaseController
    {
        private readonly IDashBoardService _dashboardService;
        public DashBoardController(IDashBoardService dashBoardService)
        {
            _dashboardService = dashBoardService;
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetToday()
        {
            var result = await _dashboardService.GetTodaySummary();
            return ToActionResult(result);
        }

        [HttpGet("line-chart")]
        public async Task<IActionResult> GetLineChart(
            [FromQuery] DateTime? From, [FromQuery] DateTime? To,
            [FromQuery] DashBoardQuery range = DashBoardQuery.Last7Days)
        {
            switch (range)
            {
                case DashBoardQuery.Custom:
                    return ToActionResult(await _dashboardService.GetDashboard(From!.Value, To!.Value));
                case DashBoardQuery.Last7Days:
                    return ToActionResult(await _dashboardService.Get7DaysDashboard());
                case DashBoardQuery.Last6Months:
                    return ToActionResult(await _dashboardService.Get6MonthsDashboard());
                default:
                    return BadRequest("Invalid range");
            }
        }

        [HttpGet("bar-chart")]
        public async Task<IActionResult> GetBarChart(
            [FromQuery] DateTime? From, [FromQuery] DateTime? To,
            [FromQuery] DashBoardQuery range = DashBoardQuery.Last7Days)
        {
            switch (range)
            {
                case DashBoardQuery.Custom:
                    return ToActionResult(await _dashboardService.GetTopBoothsInCustom(From!.Value, To!.Value));
                case DashBoardQuery.Last7Days:
                    return ToActionResult(await _dashboardService.GetTopBoothsIn7Days());
                case DashBoardQuery.Last6Months:
                    return ToActionResult(await _dashboardService.GetTopBoothsIn6Months());
                default:
                    return BadRequest("Invalid range");
            }
        }

        [HttpGet("pie-chart")]
        public async Task<IActionResult> GetPieChart(
            [FromQuery] DateTime? From, [FromQuery] DateTime? To,
            [FromQuery] DashBoardQuery range = DashBoardQuery.Last7Days)
        {
            switch (range)
            {
                case DashBoardQuery.Custom:
                    return ToActionResult(await _dashboardService.GetPaymentMethodInCustom(From!.Value, To!.Value));
                case DashBoardQuery.Last7Days:
                    return ToActionResult(await _dashboardService.GetPaymentMethodIn7Days());
                case DashBoardQuery.Last6Months:
                    return ToActionResult(await _dashboardService.GetPaymentMethodIn6Months());
                default:
                    return BadRequest("Invalid range");
            }
        }
    }
}
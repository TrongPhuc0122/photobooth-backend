using Application.Interfaces;
using Application.Services;
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
        [HttpGet]
        public async Task<IActionResult> GetDashBoard([FromQuery] DateTime? From,[FromQuery] DateTime? To, [FromQuery] DashBoardQuery range = DashBoardQuery.Last7Days)
        {
            ServiceResult<DashBoardDto> result;
            switch (range)
            {
                case DashBoardQuery.Custom:
                    result = await _dashboardService.GetDashboard(From!.Value, To!.Value);
                    break;
                case DashBoardQuery.Last7Days:
                    result = await _dashboardService.Get7DaysDashboard();
                    break;
                case DashBoardQuery.Last6Months:
                    result = await _dashboardService.Get6MonthsDashboard();
                    break;
                default:
                    return BadRequest("Invalid range");
            }
            return ToActionResult(result);
        }
    }
}
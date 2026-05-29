using Application.DTOs.Commons;
using Application.DTOs.Identites.Booths;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetDashBoard([FromQuery] DateTime From,[FromQuery] DateTime To)
        {
            var result = await _dashboardService.GetDashboard(From, To);
            return ToActionResult(result);
        }
    }
}
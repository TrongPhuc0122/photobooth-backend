using Application.DTOs.Identites;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingController : BaseController
    {
        private readonly ISettingService _service;
        public SettingController(ISettingService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Config([FromBody] CreateSettingDto dto)
        {
            var result = _service.Configuration(dto);
            return ToActionResult(result);
        }
    }
}
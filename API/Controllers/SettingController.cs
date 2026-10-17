using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Identites;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingController : BaseController
    {
        private readonly ISettingService _settingService;

        public SettingController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpGet]
        public IActionResult GetSetting()
        {
            var result = _settingService.GetAll();
            return ToActionResult(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetSettingById(int id)
        {
            var result = _settingService.GetById(id);
            return ToActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSetting([FromBody] CreateSettingDto dto)
        {
            var result = await _settingService.CreateAsync(dto);
            return ToActionResult(result);
        }

        [HttpPost("booth-info")]
        public async Task<IActionResult> BoothInfo([FromBody] CreateSettingLogDto dto)
        {
            var result = await _settingService.BoothInfor(dto);
            return ToActionResult(result);
        }
    }
}
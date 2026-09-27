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
        public async Task<IActionResult> Create([FromBody] CreateSettingDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return ToActionResult(result);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _service.GetAll();
            return ToActionResult(result);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var result = _service.GetById(id);
            return ToActionResult(result);
        }
    }
}
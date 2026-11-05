using Application.DTOs.Commons;
using Application.DTOs.Identites;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusCodeController : BaseController
    {
        private readonly IStatusCodeService _statusCodeService;
        public StatusCodeController(IStatusCodeService statusCodeService)
        {
            _statusCodeService = statusCodeService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] StatusCodeQueryParameters parameters)
        {
            var result = _statusCodeService.GetAll(parameters);
            return ToActionResult(result);
        }

        [HttpGet("{code}")]
        public IActionResult GetSingle([FromRoute] string code)
        {
            var result = _statusCodeService.GetSingle(code);
            return ToActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStatusCodeDto dto)
        {
            var result = await _statusCodeService.CreateAsync(dto);
            return ToActionResult(result);
        }
    }
}
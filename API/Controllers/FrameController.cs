using Application.DTOs.Commons;
using Application.Interfaces;
using Application.DTOs.Identites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FrameController : BaseController
    {
        private readonly IFrameService _frameService;
        public FrameController(IFrameService frameService)
        {
            _frameService = frameService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] FrameQueryParameters parameters)
        {
            var result = _frameService.GetAll(parameters);
            return ToActionResult(result);
        }
        [HttpGet("{frameId:int}")]
        public IActionResult GetById([FromRoute] int frameId)
        {
            var result =  _frameService.GetById(frameId);
            return ToActionResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateFrameDto dto)
        {
            var result = await _frameService.CreateFrame(dto);
            return ToActionResult(result);
        }

        [HttpDelete("{frameId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int frameId)
        {
            var result = await _frameService.SoftDeleteFrame(frameId);
            return ToActionResult(result);
        }
        [HttpPut("{frameId:int}")]
        public async Task<IActionResult> Update([FromRoute] int frameId, [FromForm]UpdateFrameDto dto)
        {
            var result = await _frameService.UpdateFrame(frameId, dto);
            return ToActionResult(result);
        }

    }
}

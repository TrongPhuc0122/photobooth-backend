using Application.DTOs.Commons;
using Application.DTOs.Identites;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FramesController : BaseController
    {
        private readonly IFrameService _frameService;

        public FramesController(IFrameService frameService)
        {
            _frameService = frameService;
        }

        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] CommonQueryParameters parameters,
            [FromQuery] LayoutType layout = LayoutType.All)
        {
            var result = _frameService.GetAll(parameters, layout);
            return ToActionResult(result);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var result = _frameService.GetById(id);
            return ToActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFrameDto model)
        {
            var result = await _frameService.CreateAsync(model);
            return ToActionResult(result);
        }
    }
}
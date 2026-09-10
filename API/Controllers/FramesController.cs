using Application.DTOs.Commons;
using Application.Interfaces;
using Application.DTOs.Identites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Shared;
using Domain.Entities;

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
        public IActionResult GetAll([FromQuery] CommonQueryParameters parameters, LayoutType layoutType = LayoutType.All)
        {
            var result = _frameService.GetAll(parameters, layoutType);
            return ToActionResult(result);
        }
        [HttpGet("{frameId:int}")]
        public IActionResult GetById([FromRoute] int frameId)
        {
            var result =  _frameService.GetById(frameId);
            return ToActionResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFrameDto dto)
        {
            var result = await _frameService.CreateAsync(dto);
            return ToActionResult(result);
        }

        [HttpPut]
        public async Task<IActionResult> Updare([FromRoute] int frameId, CreateFrameDto dto)
        {
            var result = await _frameService.UpdateAsync(frameId, dto);
            return ToActionResult(result);
        }
        [HttpDelete("{frameId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int frameId)
        {
            var result = await _frameService.SoftDelete(frameId);
            return ToActionResult(result);
        }

    }
}

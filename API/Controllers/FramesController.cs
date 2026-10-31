using Application.DTOs.Commons;
using Application.Interfaces;
using Application.DTOs.Identites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Shared;
using Domain.Entities;
using System.Net.Http.Headers;

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
        [HttpGet("{id:int}/images")]
        public async Task<IActionResult> GetImages(int id)
        {
            var paths = _frameService.GetImagePaths(id);
            if (paths == null) return NotFound();

            using var content = new MultipartFormDataContent();
            foreach (var (name, path) in paths)
            {
                var part = new StreamContent(System.IO.File.OpenRead(path));
                part.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                content.Add(part, name, $"{name}.png");
            }

            Response.ContentType = content.Headers.ContentType!.ToString();
            await content.CopyToAsync(Response.Body);
            return new EmptyResult();
        }
        [HttpGet("metadata")]
        public IActionResult GetFrameMetadata([FromQuery] CommonQueryParameters parameters, LayoutType layoutType = LayoutType.All)
        {
          var result = _frameService.GetFrameMetadata(parameters, layoutType);
          return ToActionResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFrameDto dto)
        {
            var result = await _frameService.CreateAsync(dto);
            return ToActionResult(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromRoute] int frameId, CreateFrameDto dto)
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

        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var result = _frameService.GetAllOptions();
            return ToActionResult(result);
        }
    }
}

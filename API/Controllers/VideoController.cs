using Application.DTOs.Identites;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoController : BaseController
    {
        private readonly IVideoService _videoService;
        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }
        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> Upload([FromForm] CreateVideoDto dto, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Thiếu file video");
            }

            await using var stream = file.OpenReadStream();
            var result = await _videoService.UploadAsync(dto, stream);
            return ToActionResult(result);
        }
    }

    
}
using Application.DTOs.Identites;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotoController : BaseController
    {
        private readonly IPhotoServices _photoService;
        public PhotoController(IPhotoServices photoServices)
        {
            _photoService = photoServices;
        }
        [HttpPost]
        public async Task<IActionResult> GetImage([FromBody] CreatePhotoDto dto)
        {
            var result = await _photoService.CreatePhoto(dto);
            return ToActionResult(result);
        }
    }
}
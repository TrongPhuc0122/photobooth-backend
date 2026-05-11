using Application.DTOs.Identites.Booths;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoothsController : BaseController
    {
        private readonly IBoothService _boothsService;

        public BoothsController(IBoothService boothsService)
        {
            _boothsService = boothsService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _boothsService.GetAll();
            return ToActionResult(result);
        }

        [HttpGet("{boothId:int}")]
        public IActionResult GetById([FromRoute] int boothId)
        {
            var result = _boothsService.GetById(boothId);
            return ToActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBoothDto model)
        {
            var result = await _boothsService.CreateAsync(model);
            return ToActionResult(result);
        }

        [HttpPut("{boothId:int}")]
        public async Task<IActionResult> Update([FromRoute] int boothId, [FromBody] CreateBoothDto model)
        {
            var result = await _boothsService.UpdateAsync(boothId, model);
            return ToActionResult(result);
        }

        [HttpDelete("{boothId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int boothId)
        {
            var result = await _boothsService.Delete(boothId);
            return ToActionResult(result);
        }
        // ===== BoothError =====
        [HttpPost("{boothId:int}/error")]
        public async Task<IActionResult> CreateError(int boothId, [FromBody] CreateBoothErrorDto dto)
        {
            var result = await _boothsService.CreateError(boothId, dto);
            return ToActionResult(result);
        }

        [HttpGet("{boothId:int}/errors")]
        public IActionResult GetErrors([FromRoute] int boothId, [FromQuery] bool activeOnly = false)
        {
            if (activeOnly)
            {
                var result = _boothsService.GetActiveErrors(boothId);
                return ToActionResult(result);
            }
            else
            {
                var result = _boothsService.GetAllErrors(boothId);
                return ToActionResult(result);
            }
        }

        [HttpPut("error/fix")]
        public async Task<IActionResult> FixError([FromQuery] int boothId, [FromQuery] string cause)
        {
            var result = await _boothsService.FixError(boothId, cause);
            return ToActionResult(result);
        }

        // ===== BoothResources =====
        [HttpPut("{boothId:int}/resources")]
        public async Task<IActionResult> UpdateResources(int boothId, [FromQuery] int? paper, [FromQuery] int? ribbon)
        {
            var result = await _boothsService.UpdateResources(boothId, paper, ribbon);
            return ToActionResult(result);
        }

        [HttpPut("{boothId:int}/resources/storage")]
        public async Task<IActionResult> SetBoothStorage(int boothId, [FromQuery] int? paperMax, [FromQuery] int? ribbonMax)
        {
            var result = await _boothsService.SetBoothStorage(boothId, paperMax, ribbonMax);
            return ToActionResult(result);
        }
    }
}

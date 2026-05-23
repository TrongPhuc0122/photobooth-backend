using Application.DTOs.Commons;
using Application.DTOs.Identites.Booths;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        #region Booths
        [HttpGet]
        public IActionResult GetAll([FromQuery] CommonQueryParameters parameters)
        {
            var result = _boothsService.GetAll(parameters);
            return ToActionResult(result);
        }

        [HttpGet("{boothId:Guid}")]
        public IActionResult GetById([FromRoute] Guid boothId)
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

        [HttpPut("{boothId:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid boothId, [FromBody] CreateBoothDto model)
        {
            var result = await _boothsService.UpdateAsync(boothId, model);
            return ToActionResult(result);
        }

        [HttpDelete("{boothId:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid boothId)
        {
            var result = await _boothsService.Delete(boothId);
            return ToActionResult(result);
        }
        #endregion

        #region  BoothError
        [HttpPost("{boothId:Guid}/error")]
        public async Task<IActionResult> CreateError(Guid boothId, [FromBody] CreateBoothErrorDto dto)
        {
            var result = await _boothsService.CreateError(boothId, dto);
            return ToActionResult(result);
        }

        [HttpGet("{boothId:Guid}/errors")]
        public IActionResult GetErrors([FromRoute] Guid boothId, [FromQuery] bool activeOnly = false)
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
        public async Task<IActionResult> FixError([FromQuery] Guid boothId, [FromQuery] string cause)
        {
            var result = await _boothsService.FixError(boothId, cause);
            return ToActionResult(result);
        }
        #endregion

        #region  BoothsResources
        [HttpPut("{boothId:Guid}/resources")]
        public async Task<IActionResult> UpdateResources(Guid boothId, [FromQuery] int? paper, [FromQuery] int? ribbon)
        {
            var result = await _boothsService.UpdateResources(boothId, paper, ribbon);
            return ToActionResult(result);
        }

        [HttpPut("{boothId:GUid}/resources/storage")]
        public async Task<IActionResult> SetBoothStorage(Guid boothId, [FromQuery] int? paperMax, [FromQuery] int? ribbonMax)
        {
            var result = await _boothsService.SetBoothStorage(boothId, paperMax, ribbonMax);
            return ToActionResult(result);
        }
        #endregion

        #region BoothHealth
        [HttpPost("{boothId:Guid}/heartbeat")]
        public async Task<IActionResult> GetHeartBeat (Guid boothId)
        {
            var result = await _boothsService.GetHeartBeat(boothId);
            return ToActionResult(result);
        }
        #endregion
    }
}

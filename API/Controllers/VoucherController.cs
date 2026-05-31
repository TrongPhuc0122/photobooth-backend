using Application.DTOs.Commons;
using Application.DTOs.Identites;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoucherController : BaseController
    {
        private readonly IVoucherService _voucherService;
        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }
        [HttpGet]
        public IActionResult GetAll([FromQuery] VoucherQueryParameters parameters)
        {
            var result = _voucherService.GetAll(parameters);
            return ToActionResult(result);
        }

        [HttpGet("{voucherId:int}")]
        public IActionResult GetById([FromRoute] int voucherId)
        {
            var result = _voucherService.GetById(voucherId);
            return ToActionResult(result);
        }
        [HttpGet("{voucherCode}")]
        public IActionResult GetVoucherCode([FromRoute] string voucherCode)
        {
            var result = _voucherService.GetVoucherCode(voucherCode);
            return ToActionResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> Creater([FromBody] CreateVoucherDto model)
        {
            var result = await _voucherService.CreateAsync(model);
            return ToActionResult(result);
        }
        [HttpPut("{voucherId:int}")]
        public async Task<IActionResult> Update([FromRoute] int voucherId, [FromBody] CreateVoucherDto model)
        {
            var result = await _voucherService.UpdateAsync(voucherId, model);
            return ToActionResult(result);
        }
        [HttpDelete("{voucherId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int voucherId)
        {
            var result = await _voucherService.SoftDelete(voucherId);
            return ToActionResult(result);
        }
    }
}
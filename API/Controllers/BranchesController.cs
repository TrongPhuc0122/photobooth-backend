using Application.DTOs.Commons;
using Application.DTOs.Identites;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Context.Repositories;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchesController : BaseController
    {
        private readonly IBranchService _branchService;

        public BranchesController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery]BranchQueryParameters parameters)
        {
            var result = _branchService.GetAll(parameters);
            return ToActionResult(result);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var result = _branchService.GetById(id);
            return ToActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBranchDto model)
        {
            var result = await _branchService.CreateAsync(model);
            return ToActionResult(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CreateBranchDto model)
        {
            var result = await _branchService.UpdateAsync(id, model);
            return ToActionResult(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _branchService.SoftDelete(id);
            return ToActionResult(result);
        }
        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var result = _branchService.GetAllOptions();
            return ToActionResult(result);
        }
    }
}

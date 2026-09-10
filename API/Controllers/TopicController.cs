using Application.DTOs;
using Application.DTOs.Commons;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicsController : BaseController
    {
        private readonly ITopicService _topicService;

        public TopicsController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] TopicQueryParameters parameters)
        {
            var result = _topicService.GetAll(parameters);
            return ToActionResult(result);
        }

        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var result = _topicService.GetAllOptions();
            return ToActionResult(result);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var result = _topicService.GetById(id);
            return ToActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTopicDto model)
        {
            var result = await _topicService.CreateAsync(model);
            return ToActionResult(result);
        }
        
        [HttpPut]
        public async Task<IActionResult> Update([FromRoute] int topicId, [FromBody] CreateTopicDto dto)
        {
            var result = await _topicService.UpdateAsync(topicId, dto);
            return ToActionResult(result);
        }
        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _topicService.SoftDelete(id);
            return ToActionResult(result);
        }
    }
}
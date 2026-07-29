using Application.DTOs;
using Application.DTOs.Commons;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetAll([FromQuery] CommonQueryParameters parameters)
        {
            var result = _topicService.GetAll(parameters);
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
    }
}
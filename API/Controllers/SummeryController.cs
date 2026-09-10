using Application.DTOs.Identites;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class SummeryController : BaseController
{
    private readonly ISummeryService _summeryService;

    public SummeryController(ISummeryService summeryService)
    {
        _summeryService = summeryService;
    }

    [HttpGet]
    public IActionResult GetSummeryInfor()
    {
        var result = _summeryService.GetSummeryInfor();
        return ToActionResult(result);
    }
}
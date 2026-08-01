using Application.DTOs.Commons;
using Application.DTOs.Identites;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : BaseController
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] InvoiceQueryParameters parameters)
        {
            var result = _invoiceService.GetAll(parameters);
            return ToActionResult(result);
        }

        [HttpGet("detail")]
        public IActionResult GetDetailAll([FromQuery] InvoiceQueryParameters parameters)
        {
            var result = _invoiceService.GetDetailAll(parameters);
            return ToActionResult(result);
        }

        [HttpGet("{invoiceId:int}")]
        public IActionResult GetById([FromRoute] int invoiceId)
        {
            var result = _invoiceService.GetById(invoiceId);
            return ToActionResult(result);
        }

        [HttpGet("detail/{invoiceId:int}")]
        public IActionResult GetDetailById([FromRoute] int invoiceId)
        {
            var result = _invoiceService.GetDetailById(invoiceId);
            return ToActionResult(result);
        }

        [HttpPost("{boothId:Guid}")]
        public async Task<IActionResult> Create([FromRoute] Guid boothId, [FromBody] CreateInvoicesDto model)
        {
            var result = await _invoiceService.CreateAsync(boothId, model);
            return ToActionResult(result);
        }
    }
}
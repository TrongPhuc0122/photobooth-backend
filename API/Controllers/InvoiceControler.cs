using Application.DTOs.Commons;
using Application.DTOs.Identites;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

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
        public IActionResult GetAll([FromQuery] InvoiceQueryParameters parameters, bool detail=false)
        {
            if (!detail)
            {
                var result = _invoiceService.GetAll(parameters);
                return ToActionResult(result);
            }
            else
            {
                var result = _invoiceService.GetDetailAll(parameters);
                return ToActionResult(result);
            }  
        }
        [HttpGet("{invoiceId:int}")]
        public IActionResult GetDetailById(int invoiceId)
        {
            var result = _invoiceService.GetDetailById(invoiceId);
            return ToActionResult(result);
        }
        [HttpPost("{boothId:Guid}")]
        public async Task<IActionResult> Create(Guid boothId, [FromBody] CreateInvoicesDto dto)
        {
            var result = await _invoiceService.CreateAsync(boothId, dto);
            return ToActionResult(result);
        }
    }
}
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

        [HttpPost("{boothId}")]
        public async Task<IActionResult> Create(int boothId, [FromBody] CreateInvoicesDto dto)
        {
            var result = await _invoiceService.CreateAsync(boothId, dto);
            return ToActionResult(result);
        }

        [HttpGet]
        public IActionResult GetByAdmin(
            [FromQuery] string? branchCode,
            [FromQuery] string? boothName,
            [FromQuery] string from,
            [FromQuery] string to)
        {
            if (!DateTime.TryParseExact(from, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var fromDate) ||
                !DateTime.TryParseExact(to, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var toDate))
            {
                return BadRequest("Định dạng ngày không hợp lệ. Vui lòng nhập dd/MM/yyyy");
            }

            var result = _invoiceService.GetByAdmin(branchCode, boothName, fromDate, toDate);
            return ToActionResult(result);
        }
    }
}
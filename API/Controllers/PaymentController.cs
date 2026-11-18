using Application.DTOs.Identites.Payment;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> PushAmount([FromBody] CreatePaymentDto dto)
        {
            var result = await _paymentService.CreateAsync(dto);
            return ToActionResult(result);
        }
    }
}
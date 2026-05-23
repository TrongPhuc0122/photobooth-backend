using Shared;

namespace Application.DTOs.Identites
{
    public class CreateInvoicesDto
    {
        public required decimal Price { get; set; }
        public string? VoucherCode { get; set; }
        public required PaymentMethodStatus PaymentMethod { get; set; }
    }
}
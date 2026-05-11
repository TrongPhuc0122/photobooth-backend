namespace Application.DTOs.Identites
{
    public class CreateInvoicesDto
    {
        public required decimal Price { get; set; }
        public string? VoucherCode { get; set; }
        public required string PaymentMethod { get; set; } = string.Empty;
    }
}
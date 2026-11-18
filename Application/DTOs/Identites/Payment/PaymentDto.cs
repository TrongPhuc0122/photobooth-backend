using Shared;

namespace Application.DTOs.Identites.Payment;

public class PaymentDto
{
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime RecievedAt { get; set; }
    public required string QR { get; set; }
    public string? Message { get; set; }
    
}
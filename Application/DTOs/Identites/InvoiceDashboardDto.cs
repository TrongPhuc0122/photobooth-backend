using Shared;

namespace Application.DTOs.Identites;

public class InvoiceDashboardDto
{
    public decimal FinalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public PaymentMethodStatus PaymentMethod { get; set; }
    public Guid BoothId { get; set; }
}
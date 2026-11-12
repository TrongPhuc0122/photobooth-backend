namespace Application.DTOs.Identites;

public class CreateVideoDto
{
    public required Guid BoothId { get; set; }
    public required string InvoiceCode { get; set; }
}
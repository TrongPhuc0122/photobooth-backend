namespace Application.DTOs.Identites;

public class VideoDto
{
    public Guid BoothId { get; set; }
    public string InvoiceCode { get; set; } = string.Empty;
    public string VideoPath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
namespace Application.DTOs.Identites;
public class CreatePhotoDto
{
    public Guid BoothId { get; set; }
    public string Base64Image { get; set; } = string.Empty;
}
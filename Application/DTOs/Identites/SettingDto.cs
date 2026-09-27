namespace Application.DTOs.Identites;

public class SettingDto
{
    public int SettingId { get; set; }
    public required CameraDto Camera { get; set; }
    public required PrinterDto Printer { get; set; }
    public required SystemDto System { get; set; }
    public List<Guid> BoothIds { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
}

namespace Application.DTOs.Identites;

public class SettingDto
{
    public int SettingId { get; set; }
    public CameraSettingDto Camera { get; set; } = new();
    public PrinterSettingDto Printer { get; set; } = new();
    public SystemSettingDto System { get; set; } = new();
    public DateTime UpdateAt { get; set; }
    public DateTime SettingTime { get; set; }
}

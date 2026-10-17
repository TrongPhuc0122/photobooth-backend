namespace Application.DTOs.Identites;

public class SettingLogDto
{
    public Guid BoothId { get; set; }
    public int SettingId { get; set; }
    public DateTime SettingTime { get; set; }
    public bool SettingStatus { get; set; }
    public DateTime CreateAt { get; set; }
}
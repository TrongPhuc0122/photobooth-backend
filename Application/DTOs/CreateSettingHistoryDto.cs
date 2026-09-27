using Domain.Entities;

public class CreateSettingHistoryDto
{
    public required CameraSetting Camera { get; set; }
    public required PrinterSetting Printer { get; set; }
    public required SystemSetting System { get; set; }
    public DateTime CalledAt { get; set; }
}
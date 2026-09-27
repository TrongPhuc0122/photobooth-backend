using Domain.Entities;

namespace Application.DTOs.Identites;

public class CreateBoothSettingDto
{
    public int SettingId { get; set; }
    public required CameraSetting Camera { get; set; }
    public required PrinterSetting Printer { get; set; }
    public required SystemSetting System { get; set; }
    public DateTime CalledAt { get; set; }

}
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;
public class SettingHistory : IHasSettingGroups
{
    [Key] public int SettingHistoryId { get; set; }
    public Guid BoothId { get; set; }
    public Booths Booth { get; set; } = null!;
    public required CameraSetting Camera { get; set; }
    public required PrinterSetting Printer { get; set; }
    public required SystemSetting System { get; set; }
    public DateTime CalledAt { get; set; }
}
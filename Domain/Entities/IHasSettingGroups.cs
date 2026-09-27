namespace Domain.Entities;

public interface IHasSettingGroups
{
    CameraSetting Camera { get; set; }
    PrinterSetting Printer { get; set; }
    SystemSetting System { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Setting
{
    [Key]
    public int SettingId { get; set; }
    public required CameraSetting Camera { get; set; }
    public required PrinterSetting Printer { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CameraSetting
{
    public string AE { get; set; } = string.Empty;
    public string ISO { get; set; } = string.Empty;
    public string Tv { get; set; } = string.Empty;
    public string Av { get; set; } = string.Empty;
    public string Exposure { get; set; } = string.Empty;
    public string WB { get; set; } = string.Empty;
    public string Metering { get; set; } = string.Empty;
    public string Quality { get; set; } = string.Empty;
    public string Flash { get; set; } = string.Empty;
    public string AFMode { get; set; } = string.Empty;
    public string PictureStyle { get; set; } = string.Empty;
    public string DriveMode { get; set; } = string.Empty;
}

public class PrinterSetting
{
    public bool ColorCorrection { get; set; }
    public bool AutoRotate { get; set; }
    public bool Borderless { get; set; }
    public bool HalfCut { get; set; }
    public string PrinterDriverName { get; set; } = string.Empty;
}
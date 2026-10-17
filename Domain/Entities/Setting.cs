using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;
public class Setting
{
    [Key]
    public int SettingId { get; set; }
    public required CameraSetting Camera { get; set; }
    public required PrinterSetting Printer { get; set; }
    public required SystemSetting System { get; set; }
    public DateTime UpdateAt { get; set; }
    public virtual ICollection<Booths> Booths { get; set; } = new List<Booths>();
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

public class SystemSetting
{
    public required DefaultSetting Default { get; set; }
    public required TimerSetting Timer { get; set; }
    public required CaptureSetting Capture { get; set; }
    public required AudioSetting Audio { get; set; }
}

public class DefaultSetting
{
    public string Language { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string RunMode { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public string ServerAddress { get; set; } = string.Empty;
}

public class TimerSetting
{
    public int PreviewSeconds { get; set; }
    public int SelectScreenSeconds { get; set; }
    public int SelectPhotoSeconds { get; set; }
    public int PaymentWaitSeconds { get; set; }
    public int ErrorScreenSeconds { get; set; }
    public int SelectFilterSeconds { get; set; }
    public int PaymentMethodSeconds { get; set; }
    public int UnitPricePaymentSeconds { get; set; }
    public int SelectVoucherSeconds { get; set; }
    public int PrintWaitSeconds { get; set; }
}

public class CaptureSetting
{
    public int TotalShots { get; set; }
    public int AutoCaptureCountdown { get; set; }
    public int ShotsFor4Cut { get; set; }
    public int ShotsFor8Cut { get; set; }
    public int MaxThumbnails { get; set; }
    public int EvfGcIntervalSeconds { get; set; }
    public int EvfOffMs { get; set; }
}

public class AudioSetting
{
    public bool VoiceGuide { get; set; }
    public bool BackgroundMusic { get; set; }
    public int Volume { get; set; }
    public bool PrintBackgroundMusic { get; set; }
}
namespace Application.DTOs.Identites;

public class CreateSettingDto
{
    public CameraSettingDto Camera { get; set; } = new();
    public PrinterSettingDto Printer { get; set; } = new();
    public SystemSettingDto System { get; set; } = new();
    public DateTime UpdateAt { get; set; }
}

public class CameraSettingDto
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

public class PrinterSettingDto
{
    public bool ColorCorrection { get; set; }
    public bool AutoRotate { get; set; }
    public bool Borderless { get; set; }
    public bool HalfCut { get; set; }
    public string PrinterDriverName { get; set; } = string.Empty;
}

public class SystemSettingDto
{
    public DefaultSettingDto Default { get; set; } = new();
    public TimerSettingDto Timer { get; set; } = new();
    public CaptureSettingDto Capture { get; set; } = new();
    public AudioSettingDto Audio { get; set; } = new();
}

public class DefaultSettingDto
{
    public string Language { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string RunMode { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public string ServerAddress { get; set; } = string.Empty;
}

public class TimerSettingDto
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

public class CaptureSettingDto
{
    public int TotalShots { get; set; }
    public int AutoCaptureCountdown { get; set; }
    public int ShotsFor4Cut { get; set; }
    public int ShotsFor8Cut { get; set; }
    public int MaxThumbnails { get; set; }
    public int EvfGcIntervalSeconds { get; set; }
    public int EvfOffMs { get; set; }
}

public class AudioSettingDto
{
    public bool VoiceGuide { get; set; }
    public bool BackgroundMusic { get; set; }
    public int Volume { get; set; }
    public bool PrintBackgroundMusic { get; set; }
}
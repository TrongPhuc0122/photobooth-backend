using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Setting : IHasSettingGroups
{
    [Key]
    public int SettingId { get; set; }
    public List<Booths> Booths { get; set; } = new();
    public required CameraSetting Camera { get; set; }
    public required PrinterSetting Printer { get; set; }
    public required SystemSetting System { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool SettingStatus { get; set; }
    public DateTime SettingTime { get; set; }
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
    public string Language { get; set; } = string.Empty;      // Ngôn ngữ
    public string Country { get; set; } = string.Empty;       // Quốc gia
    public string RunMode { get; set; } = string.Empty;       // Chế độ chạy
    public string Server { get; set; } = string.Empty;        // Máy chủ
    public string ServerAddress { get; set; } = string.Empty; // Địa chỉ máy chủ
}

public class TimerSetting
{
    public int PreviewSeconds { get; set; }          // Xem trước
    public int SelectScreenSeconds { get; set; }      // Màn hình chọn
    public int SelectPhotoSeconds { get; set; }       // Ảnh lựa chọn
    public int PaymentWaitSeconds { get; set; }       // Chờ thanh toán
    public int ErrorScreenSeconds { get; set; }       // Màn hình lỗi
    public int SelectFilterSeconds { get; set; }      // Chọn bộ lọc
    public int PaymentMethodSeconds { get; set; }     // Phương thức thanh toán
    public int UnitPricePaymentSeconds { get; set; }  // Thanh toán đơn giá
    public int SelectVoucherSeconds { get; set; }     // Chọn mã giảm giá
    public int PrintWaitSeconds { get; set; }         // Chờ in
}

public class CaptureSetting
{
    public int TotalShots { get; set; }             // Tổng số lượt chụp
    public int AutoCaptureCountdown { get; set; }   // Đếm ngược chụp tự động
    public int ShotsFor4Cut { get; set; }           // Số ảnh chụp đối với 4 cut
    public int ShotsFor8Cut { get; set; }           // Số ảnh chụp đối với 8 cut
    public int MaxThumbnails { get; set; }          // Số ảnh thu nhỏ tối đa
    public int EvfGcIntervalSeconds { get; set; }   // Khoảng thời gian EVF GC
    public int EvfOffMs { get; set; }               // Khoảng tắt EVF (ms)
}

public class AudioSetting
{
    public bool VoiceGuide { get; set; }          // Hướng dẫn giọng
    public bool BackgroundMusic { get; set; }     // Nhạc nền
    public int Volume { get; set; }               // Âm lượng
    public bool PrintBackgroundMusic { get; set; } // Nhạc nền in
}

public class BoothSetting
{
    public int BoothId { get; set; }
    public Booths Booth { get; set; } = null!;
    public int SettingId { get; set; }
    public Setting Setting { get; set; } = null!;
    public BoothSettingStatus Status { get; set; } = BoothSettingStatus.Pending;
    public DateTime? AckedAt { get; set; }
}

public enum BoothSettingStatus
{
    Pending,
    Applied,
    Missed
}
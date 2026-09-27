using Application.DTOs.Identites;
using Application.Interfaces;
using Application.Interfaces.Commons;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities;
using Shared.Results;

namespace Application.Services;

public class SettingService : GenericService<Setting, SettingDto, CreateSettingDto, int>, ISettingService
{
    private readonly IGenericRepository<Booths, Guid> _boothRepository;

    public SettingService(
        IGenericRepository<Booths, Guid> boothRepository,
        IGenericRepository<Setting, int> repository,
        IMapper mapper,
        IUnitOfWork unitOfWork
    ) : base(repository, mapper, unitOfWork)
    {
        _boothRepository = boothRepository;
    }

    public override async Task<ServiceResult<SettingDto>> CreateAsync(CreateSettingDto dto)
    {
        var booths = _boothRepository.GetMulti(b => dto.BoothIds.Contains(b.BoothId)).ToList();

        if (booths.Count != dto.BoothIds.Count)
        {
            return ServiceResult<SettingDto>.NotFound("Một hoặc nhiều BoothId không tồn tại");
        }

        try
        {
            var setting = new Setting
            {
                Camera = new CameraSetting
                {
                    AE = dto.Camera.AE,
                    ISO = dto.Camera.ISO,
                    Tv = dto.Camera.Tv,
                    Av = dto.Camera.Av,
                    Exposure = dto.Camera.Exposure,
                    WB = dto.Camera.WB,
                    Metering = dto.Camera.Metering,
                    Quality = dto.Camera.Quality,
                    Flash = dto.Camera.Flash,
                    AFMode = dto.Camera.AFMode,
                    PictureStyle = dto.Camera.PictureStyle,
                    DriveMode = dto.Camera.DriveMode
                },
                Printer = new PrinterSetting
                {
                    ColorCorrection = dto.Printer.ColorCorrection,
                    AutoRotate = dto.Printer.AutoRotate,
                    Borderless = dto.Printer.Borderless,
                    HalfCut = dto.Printer.HalfCut,
                    PrinterDriverName = dto.Printer.PrinterDriverName
                },
                System = new SystemSetting
                {
                    Default = new DefaultSetting
                    {
                        Language = dto.System.Default.Language,
                        Country = dto.System.Default.Country,
                        RunMode = dto.System.Default.RunMode,
                        Server = dto.System.Default.Server,
                        ServerAddress = dto.System.Default.ServerAddress
                    },
                    Timer = new TimerSetting
                    {
                        PreviewSeconds = dto.System.Timer.PreviewSeconds,
                        SelectScreenSeconds = dto.System.Timer.SelectScreenSeconds,
                        SelectPhotoSeconds = dto.System.Timer.SelectPhotoSeconds,
                        PaymentWaitSeconds = dto.System.Timer.PaymentWaitSeconds,
                        ErrorScreenSeconds = dto.System.Timer.ErrorScreenSeconds,
                        SelectFilterSeconds = dto.System.Timer.SelectFilterSeconds,
                        PaymentMethodSeconds = dto.System.Timer.PaymentMethodSeconds,
                        UnitPricePaymentSeconds = dto.System.Timer.UnitPricePaymentSeconds,
                        SelectVoucherSeconds = dto.System.Timer.SelectVoucherSeconds,
                        PrintWaitSeconds = dto.System.Timer.PrintWaitSeconds
                    },
                    Capture = new CaptureSetting
                    {
                        TotalShots = dto.System.Capture.TotalShots,
                        AutoCaptureCountdown = dto.System.Capture.AutoCaptureCountdown,
                        ShotsFor4Cut = dto.System.Capture.ShotsFor4Cut,
                        ShotsFor8Cut = dto.System.Capture.ShotsFor8Cut,
                        MaxThumbnails = dto.System.Capture.MaxThumbnails,
                        EvfGcIntervalSeconds = dto.System.Capture.EvfGcIntervalSeconds,
                        EvfOffMs = dto.System.Capture.EvfOffMs
                    },
                    Audio = new AudioSetting
                    {
                        VoiceGuide = dto.System.Audio.VoiceGuide,
                        BackgroundMusic = dto.System.Audio.BackgroundMusic,
                        Volume = dto.System.Audio.Volume,
                        PrintBackgroundMusic = dto.System.Audio.PrintBackgroundMusic
                    }
                },
                Booths = booths,
                UpdatedAt = DateTime.UtcNow,
                SettingStatus = true,
                SettingTime = DateTime.UtcNow
            };

            _repository.Add(setting);
            await _unitOfWork.SaveChangesAsync();

            foreach (var booth in booths)
            {
                booth.SettingId = setting.SettingId;
                _boothRepository.Update(booth);
            }
            await _unitOfWork.SaveChangesAsync();

            var result = MapToDto(setting);
            return ServiceResult<SettingDto>.Created(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<SettingDto>.InternalServerError($"Lỗi tạo setting: {ex.Message}");
        }
    }

    private static SettingDto MapToDto(Setting s) => new()
    {
        SettingId = s.SettingId,
        Camera = new CameraDto
        {
            AE = s.Camera.AE, ISO = s.Camera.ISO, Tv = s.Camera.Tv, Av = s.Camera.Av,
            Exposure = s.Camera.Exposure, WB = s.Camera.WB, Metering = s.Camera.Metering,
            Quality = s.Camera.Quality, Flash = s.Camera.Flash, AFMode = s.Camera.AFMode,
            PictureStyle = s.Camera.PictureStyle, DriveMode = s.Camera.DriveMode
        },
        Printer = new PrinterDto
        {
            ColorCorrection = s.Printer.ColorCorrection, AutoRotate = s.Printer.AutoRotate,
            Borderless = s.Printer.Borderless, HalfCut = s.Printer.HalfCut,
            PrinterDriverName = s.Printer.PrinterDriverName
        },
        System = new SystemDto
        {
            Default = new DefaultDto
            {
                Language = s.System.Default.Language, Country = s.System.Default.Country,
                RunMode = s.System.Default.RunMode, Server = s.System.Default.Server,
                ServerAddress = s.System.Default.ServerAddress
            },
            Timer = new TimerDto
            {
                PreviewSeconds = s.System.Timer.PreviewSeconds, SelectScreenSeconds = s.System.Timer.SelectScreenSeconds,
                SelectPhotoSeconds = s.System.Timer.SelectPhotoSeconds, PaymentWaitSeconds = s.System.Timer.PaymentWaitSeconds,
                ErrorScreenSeconds = s.System.Timer.ErrorScreenSeconds, SelectFilterSeconds = s.System.Timer.SelectFilterSeconds,
                PaymentMethodSeconds = s.System.Timer.PaymentMethodSeconds, UnitPricePaymentSeconds = s.System.Timer.UnitPricePaymentSeconds,
                SelectVoucherSeconds = s.System.Timer.SelectVoucherSeconds, PrintWaitSeconds = s.System.Timer.PrintWaitSeconds
            },
            Capture = new CaptureDto
            {
                TotalShots = s.System.Capture.TotalShots, AutoCaptureCountdown = s.System.Capture.AutoCaptureCountdown,
                ShotsFor4Cut = s.System.Capture.ShotsFor4Cut, ShotsFor8Cut = s.System.Capture.ShotsFor8Cut,
                MaxThumbnails = s.System.Capture.MaxThumbnails, EvfGcIntervalSeconds = s.System.Capture.EvfGcIntervalSeconds,
                EvfOffMs = s.System.Capture.EvfOffMs
            },
            Audio = new AudioDto
            {
                VoiceGuide = s.System.Audio.VoiceGuide, BackgroundMusic = s.System.Audio.BackgroundMusic,
                Volume = s.System.Audio.Volume, PrintBackgroundMusic = s.System.Audio.PrintBackgroundMusic
            }
        },
        BoothIds = s.Booths.Select(b => b.BoothId).ToList(),
        UpdatedAt = s.UpdatedAt
    };
}
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
    private readonly IGenericRepository<SettingLog, int> _logRepository;

    public SettingService(
        IGenericRepository<Setting, int> repository,
        IGenericRepository<Booths, Guid> boothRepository,
        IGenericRepository<SettingLog, int> logRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork
    ) : base(repository, mapper, unitOfWork)
    {
        _boothRepository = boothRepository;
        _logRepository = logRepository;
    }

    public async override Task<ServiceResult<SettingDto>> CreateAsync(CreateSettingDto dto)
    {
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
                    DriveMode = dto.Camera.DriveMode,
                },
                Printer = new PrinterSetting
                {
                    ColorCorrection = dto.Printer.ColorCorrection,
                    AutoRotate = dto.Printer.AutoRotate,
                    Borderless = dto.Printer.Borderless,
                    HalfCut = dto.Printer.HalfCut,
                    PrinterDriverName = dto.Printer.PrinterDriverName,
                },
                System = new SystemSetting
                {
                    Default = new DefaultSetting
                    {
                        Language = dto.System.Default.Language,
                        Country = dto.System.Default.Country,
                        RunMode = dto.System.Default.RunMode,
                        Server = dto.System.Default.Server,
                        ServerAddress = dto.System.Default.ServerAddress,
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
                        PrintWaitSeconds = dto.System.Timer.PrintWaitSeconds,
                    },
                    Capture = new CaptureSetting
                    {
                        TotalShots = dto.System.Capture.TotalShots,
                        AutoCaptureCountdown = dto.System.Capture.AutoCaptureCountdown,
                        ShotsFor4Cut = dto.System.Capture.ShotsFor4Cut,
                        ShotsFor8Cut = dto.System.Capture.ShotsFor8Cut,
                        MaxThumbnails = dto.System.Capture.MaxThumbnails,
                        EvfGcIntervalSeconds = dto.System.Capture.EvfGcIntervalSeconds,
                        EvfOffMs = dto.System.Capture.EvfOffMs,
                    },
                    Audio = new AudioSetting
                    {
                        VoiceGuide = dto.System.Audio.VoiceGuide,
                        BackgroundMusic = dto.System.Audio.BackgroundMusic,
                        Volume = dto.System.Audio.Volume,
                        PrintBackgroundMusic = dto.System.Audio.PrintBackgroundMusic,
                    },
                },
                UpdateAt = dto.UpdateAt,
            };
            _repository.Add(setting);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<SettingDto>.Success(_mapper.Map<SettingDto>(setting));
        }
        catch (Exception ex)
        {
            return ServiceResult<SettingDto>.InternalServerError($"Fail to post setting {ex.Message}");
        }
    }

    public async Task<ServiceResult> BoothInfor(CreateSettingLogDto dto)
    {
        try
        {
            Booths booth = _boothRepository.GetSingleById(dto.BoothId);
            Setting setting = _repository.GetSingleById(dto.SettingId);
            DateTime now = DateTime.UtcNow;

            var log = new SettingLog
            {
                BoothId = booth.BoothId,
                SettingId = setting.SettingId,
                SettingTime = now,
                SettingStatus = true,
                CreateAt = now,
            };

            _logRepository.Add(log);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Success();
        }
        catch (KeyNotFoundException)
        {
            return ServiceResult.NotFound($"Booth id={dto.BoothId} or Setting id={dto.SettingId} doesn't exist");
        }
    }
}
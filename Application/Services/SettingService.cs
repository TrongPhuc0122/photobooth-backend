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

    public ServiceResult<SettingDto> Configuration(CreateSettingDto dto)
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
                UpdatedAt = DateTime.UtcNow
            };

            _repository.Add(setting);
            _unitOfWork.SaveChangesAsync();

            var result = new SettingDto
            {
                Status = false // chưa áp dụng, chờ background job đẩy xuống booth theo lịch
            };

            return ServiceResult<SettingDto>.Created(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<SettingDto>.InternalServerError($"Lỗi lưu cấu hình: {ex.Message}");
        }
    }
}
using Application.DTOs.Commons;
using Application.DTOs.Identites;
using Application.Interfaces;
using Application.Interfaces.Commons;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.Results;
using SixLabors.ImageSharp;

namespace Application.Services;

public class FrameService : GenericService<Frame, FrameDto, CreateFrameDto, int>, IFrameService
{
    private readonly IGenericRepository<Branch, int> _branchRepository;
    private readonly IConfiguration _configuration;

    private const int RequiredWidth = 1200;
    private const int RequiredHeight = 1800;

    public FrameService(
        IGenericRepository<Frame, int> repository,
        IGenericRepository<Branch, int> branchRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IConfiguration configuration
    ) : base(repository, mapper, unitOfWork)
    {
        _branchRepository = branchRepository;
        _configuration = configuration;
    }

    public override async Task<ServiceResult<FrameDto>> CreateAsync(CreateFrameDto dto)
    {
        if (dto.BranchId.HasValue)
        {
            try
            {
                _branchRepository.GetSingleById(dto.BranchId.Value);
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult<FrameDto>.NotFound($"Không tồn tại BranchId = {dto.BranchId}");
            }
        }

        byte[] subjectBytes, backgroundBytes, overlayBytes;
        try
        {
            subjectBytes = Convert.FromBase64String(CleanBase64(dto.SubjectImage));
            backgroundBytes = Convert.FromBase64String(CleanBase64(dto.Background));
            overlayBytes = Convert.FromBase64String(CleanBase64(dto.Overlay));
        }
        catch (FormatException)
        {
            return ServiceResult<FrameDto>.ValidationError("Dữ liệu base64 không hợp lệ");
        }

        var subjectCheck = ValidateImageDimension(subjectBytes, "SubjectImage");
        if (subjectCheck != null) return ServiceResult<FrameDto>.ValidationError(subjectCheck);

        var backgroundCheck = ValidateImageDimension(backgroundBytes, "Background");
        if (backgroundCheck != null) return ServiceResult<FrameDto>.ValidationError(backgroundCheck);

        var overlayCheck = ValidateImageDimension(overlayBytes, "Overlay");
        if (overlayCheck != null) return ServiceResult<FrameDto>.ValidationError(overlayCheck);

        try
        {
            // B1: Tạo Frame trước (chưa có URL ảnh) để lấy FrameId
            var frame = new Frame
            {
                BranchId = dto.BranchId,
                Branchname = dto.BranchName,
                FrameName = dto.FrameName,
                TopicId = dto.TopicId,
                LayoutType = dto.LayoutType,
                SubjectImageUrl = string.Empty,
                BackgroundUrl = string.Empty,
                OverlayUrl = string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            _repository.Add(frame);
            await _unitOfWork.SaveChangesAsync(); // -> có frame.FrameId

            // B2: Lưu ảnh vào Frame/{FrameId}/...
            var subjectUrl = await SaveImageAsync(frame.FrameId, "subject", subjectBytes);
            var backgroundUrl = await SaveImageAsync(frame.FrameId, "background", backgroundBytes);
            var overlayUrl = await SaveImageAsync(frame.FrameId, "overlay", overlayBytes);

            // B3: Update lại URL rồi save lần 2
            frame.SubjectImageUrl = subjectUrl;
            frame.BackgroundUrl = backgroundUrl;
            frame.OverlayUrl = overlayUrl;

            await _unitOfWork.SaveChangesAsync();

            var result = new FrameDto
            {
                FrameId = frame.FrameId,
                BranchId = frame.BranchId,
                BranchName = frame.Branchname,
                FrameName = frame.FrameName,
                TopicId = frame.TopicId,
                LayoutType = frame.LayoutType,
                SubjectImageUrl = frame.SubjectImageUrl,
                BackgroundUrl = frame.BackgroundUrl,
                OverlayUrl = frame.OverlayUrl,
                CreatedAt = frame.CreatedAt
            };

            return ServiceResult<FrameDto>.Created(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<FrameDto>.InternalServerError($"Lỗi tạo Frame: {ex.Message}");
        }
    }

    public ServiceResult<PagedResult<FrameDto>> GetAll(CommonQueryParameters parameters, LayoutType layout)
    {
        try
        {
            string[] includes = { "Branch", "Topic" };

            var frames = _repository.GetMulti(
                f => (layout == LayoutType.All || f.LayoutType == layout)
                && (string.IsNullOrEmpty(parameters.Search) || f.FrameName.Contains(parameters.Search)),
                includes: includes
            );

            var totalCount = frames.Count();

            var pagedFrames = frames
                .Skip((parameters.Index - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToList();

            var result = pagedFrames.Select(f => new FrameDto
            {
                FrameId = f.FrameId,
                BranchId = f.BranchId,
                BranchName = f.Branch?.BranchName ?? f.Branchname ?? string.Empty,
                FrameName = f.FrameName,
                TopicId = f.TopicId,
                TopicName = f.Topic?.TopicName ?? string.Empty,
                LayoutType = f.LayoutType,
                SubjectImageUrl = f.SubjectImageUrl,
                BackgroundUrl = f.BackgroundUrl,
                OverlayUrl = f.OverlayUrl,
                CreatedAt = f.CreatedAt
            });

            var pagedResult = new PagedResult<FrameDto>(
                result,
                totalCount,
                parameters.Index,
                parameters.PageSize
            );

            return ServiceResult<PagedResult<FrameDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return ServiceResult<PagedResult<FrameDto>>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
        }
    }

    public override ServiceResult<FrameDto> GetById(int id)
    {
        Frame frame;
        try
        {
            string[] includes = { "Branch", "Topic" };
            frame = _repository.GetSingleByCondition(
                f => f.FrameId == id,
                includes
            );
        }
        catch (KeyNotFoundException)
        {
            return ServiceResult<FrameDto>.NotFound($"Không tồn tại FrameId = {id}");
        }
        try
        {
            var dto = new FrameDto
            {
                FrameId = frame.FrameId,
                BranchId = frame.BranchId,
                BranchName = frame.Branch?.BranchName ?? frame.Branchname ?? string.Empty,
                FrameName = frame.FrameName,
                TopicId = frame.TopicId,
                TopicName = frame.Topic?.TopicName ?? string.Empty,
                LayoutType = frame.LayoutType,
                SubjectImageUrl = frame.SubjectImageUrl,
                BackgroundUrl = frame.BackgroundUrl,
                OverlayUrl = frame.OverlayUrl,
                CreatedAt = frame.CreatedAt
            };
            return ServiceResult<FrameDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return ServiceResult<FrameDto>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
        }
    }

    private static string CleanBase64(string base64)
    {
        var commaIndex = base64.IndexOf(',');
        return commaIndex >= 0 ? base64[(commaIndex + 1)..] : base64;
    }

    private static string? ValidateImageDimension(byte[] imageBytes, string fieldName)
    {
        try
        {
            using var image = Image.Load(imageBytes);
            if (image.Width != RequiredWidth || image.Height != RequiredHeight)
            {
                return $"{fieldName} phải có kích thước {RequiredWidth}x{RequiredHeight}, " +
                       $"ảnh hiện tại là {image.Width}x{image.Height}";
            }
            return null;
        }
        catch (UnknownImageFormatException)
        {
            return $"{fieldName} không đúng định dạng ảnh (png, jpg, ...)";
        }
    }

    // folder: Frame/{frameId}/{imageType}.png
    private async Task<string> SaveImageAsync(int frameId, string imageType, byte[] imageBytes)
    {
        var storagePath = _configuration["FrameStorage:Path"]
            ?? throw new InvalidOperationException("Thiếu cấu hình FrameStorage:Path");
        var baseUrl = _configuration["FrameStorage:BaseUrl"]
            ?? throw new InvalidOperationException("Thiếu cấu hình FrameStorage:BaseUrl");

        var rootPath = Path.IsPathRooted(storagePath)
            ? storagePath
            : Path.Combine(Directory.GetCurrentDirectory(), storagePath);

        var folderPath = Path.Combine(rootPath, frameId.ToString());

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileName = $"{imageType}.png";
        var filePath = Path.Combine(folderPath, fileName);

        await File.WriteAllBytesAsync(filePath, imageBytes);

        return $"{baseUrl}/Frame/{frameId}/{fileName}";
    }
}
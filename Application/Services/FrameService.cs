using Application.DTOs.Commons;
using Application.DTOs.Identites;
using Application.Interfaces;
using Application.Interfaces.Commons;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Shared.Results;
using System.Drawing;

namespace Application.Services;

public class FrameService : GenericService<Frame, FrameDto, CreateFrameDto, int>, IFrameService
{
    private const int RequiredWidth = 1600;
    private const int RequiredHeight = 1800;

    private readonly IGenericRepository<Branch, int> _branchRepository;
    private readonly string _imagePath;
    private readonly string _baseUrl;

    public FrameService(
        IGenericRepository<Branch, int> branchRepository,
        IGenericRepository<Frame, int> repository,
        IConfiguration configuration,
        IMapper mapper,
        IUnitOfWork unitOfWork
    ) : base(repository, mapper, unitOfWork)
    {
        _branchRepository = branchRepository;
        _imagePath = configuration["FrameStorage:Path"]!;
        _baseUrl = configuration["FrameStorage:BaseUrl"]!;
    }

    public async Task<ServiceResult<FrameDto>> CreateFrame(CreateFrameDto dto)
    {
        if (dto.Background == null || dto.Overlay == null || dto.SubjectImage == null || string.IsNullOrWhiteSpace(dto.FrameName))
        {
            return ServiceResult<FrameDto>.ValidationError("Thiếu thông tin frame");
        }

        var sizeValidation = ValidateImageSizes(
            dto.SubjectImage,
            dto.Background,
            dto.Overlay);
        if (sizeValidation != null)
            return sizeValidation;

        try
        {
            var frame = new Frame
            {
                BranchId = dto.BranchId,
                Branchname = dto.BranchName,
                FrameName = dto.FrameName,
                TopicId = dto.TopicId,
                TopicName = dto.TopicName,
                CreatedAt = DateTime.UtcNow
            };
            _repository.Add(frame);
            await _unitOfWork.SaveChangesAsync();

            var baseUrl = $"frame/{frame.FrameId}/";

            frame.SubjectImageUrl = baseUrl + "SubjectImage.png";
            frame.BackgroundUrl = baseUrl + "Background.png";
            frame.OverlayUrl = baseUrl + "Overlay.png";
            await _unitOfWork.SaveChangesAsync();

            var folder = Path.Combine(_imagePath, frame.FrameId.ToString());
            Directory.CreateDirectory(folder);
            await SaveFileAsync(dto.SubjectImage, Path.Combine(folder, "SubjectImage.png"));
            await SaveFileAsync(dto.Background, Path.Combine(folder, "Background.png"));
            await SaveFileAsync(dto.Overlay, Path.Combine(folder, "Overlay.png"));

            return ServiceResult<FrameDto>.Success(MapToDto(frame));
        }
        catch (Exception ex)
        {
            return ServiceResult<FrameDto>.InternalServerError($"Lỗi tạo frame {ex.Message}");
        }
    }

    public ServiceResult<PagedResult<FrameDto>> GetAll(FrameQueryParameters parameters)
    {
        try
        {
            var genericParams = parameters.ToGenericQueryParameters();
            string[] searchProperties = { "Branchname" };
            string[] includes = { "Branch" };

            var pagedEntities = _repository.GetPaged(genericParams, searchProperties, includes);
            var result = pagedEntities.Items.Select(MapToDto);
            var pagedResult = new PagedResult<FrameDto>(
                result,
                pagedEntities.TotalCount,
                pagedEntities.Index,
                pagedEntities.PageSize
            );
            return ServiceResult<PagedResult<FrameDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return ServiceResult<PagedResult<FrameDto>>.InternalServerError($"Lỗi lấy danh sách frame {ex.Message}");
        }
    }

    public override ServiceResult<FrameDto> GetById(int frameId)
    {
        try
        {
            var frame = _repository.GetSingleById(frameId);
            return ServiceResult<FrameDto>.Success(MapToDto(frame));
        }
        catch (KeyNotFoundException)
        {
            return ServiceResult<FrameDto>.NotFound($"Không tồn tại FrameId = {frameId}");
        }
    }

    public async Task<ServiceResult<FrameDto>> UpdateFrame(int frameId, UpdateFrameDto dto)
    {
        try
        {
            var frame = _repository.GetSingleById(frameId);

            var sizeValidation = ValidateImageSizes(
                dto.SubjectImage,
                dto.Background,
                dto.Overlay);
            if (sizeValidation != null)
                return sizeValidation;

            frame.BranchId = dto.BranchId;
            frame.Branchname = dto.BranchName;
            frame.FrameName = dto.FrameName;
            frame.FrameId = dto.FrameId;
            frame.TopicId = dto.TopicId;
            frame.TopicName = dto.TopicName;

            var folder = Path.Combine(_imagePath, frame.FrameId.ToString());
            Directory.CreateDirectory(folder);

            if (dto.SubjectImage != null)
            {
                await SaveFileAsync(dto.SubjectImage, Path.Combine(folder, "SubjectImage.png"));
                frame.SubjectImageUrl = $"frame/{frame.FrameId}/SubjectImage.png";
            }

            if (dto.Background != null)
            {
                await SaveFileAsync(dto.Background, Path.Combine(folder, "Background.png"));
                frame.BackgroundUrl = $"frame/{frame.FrameId}/Background.png";
            }

            if (dto.Overlay != null)
            {
                await SaveFileAsync(dto.Overlay, Path.Combine(folder, "Overlay.png"));
                frame.OverlayUrl = $"frame/{frame.FrameId}/Overlay.png";
            }

            _repository.Update(frame);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<FrameDto>.Success(MapToDto(frame));
        }
        catch (KeyNotFoundException)
        {
            return ServiceResult<FrameDto>.NotFound($"Không tồn tại FrameId = {frameId}");
        }
        catch (Exception ex)
        {
            return ServiceResult<FrameDto>.InternalServerError($"Lỗi chỉnh sửa {ex.Message}");
        }
    }

    public async Task<ServiceResult> SoftDeleteFrame(int frameId)
    {
        try
        {
            _repository.SoftDelete(frameId);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.NoContent();
        }
        catch (KeyNotFoundException)
        {
            return ServiceResult.NotFound($"Không tồn tại FrameId = {frameId}");
        }
        catch (Exception ex)
        {
            return ServiceResult.InternalServerError($"Lỗi xóa frame: {ex.Message}");
        }
    }

    private static FrameDto MapToDto(Frame frame) => new()
    {
        BranchId = frame.BranchId,
        BranchName = frame.Branchname,
        FrameName = frame.FrameName,
        FrameId = frame.FrameId,
        Topic = frame.TopicId.HasValue
            ? new FrameTopicDto { TopicId = frame.TopicId.Value, TopicName = frame.TopicName ?? string.Empty }
            : null,
        SubjectImageUrl = frame.SubjectImageUrl,
        BackgroundUrl = frame.BackgroundUrl,
        OverlayUrl = frame.OverlayUrl,
        CreatedAt = frame.CreatedAt
    };

    private static ServiceResult<FrameDto>? ValidateImageSizes(params IFormFile?[] files)
    {
        foreach (var file in files)
        {
            if (file == null)
                continue;

            var validation = ValidateImageSize(file);
            if (validation != null)
                return validation;
        }

        return null;
    }

    private static ServiceResult<FrameDto>? ValidateImageSize(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var image = Image.FromStream(stream);

            if (image.Width != RequiredWidth || image.Height != RequiredHeight)
            {
                return ServiceResult<FrameDto>.ValidationError(
                    $"Ảnh '{file.FileName}' phải có kích thước {RequiredWidth}x{RequiredHeight}px (hiện tại: {image.Width}x{image.Height}px)");
            }
        }
        catch (Exception)
        {
            return ServiceResult<FrameDto>.ValidationError($"File '{file.FileName}' không phải là ảnh hợp lệ");
        }

        return null;
    }

    private static async Task SaveFileAsync(IFormFile file, string path)
    {
        await using var stream = File.Create(path);
        await file.CopyToAsync(stream);
    }
}

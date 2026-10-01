using System.Linq.Expressions;
using Application.DTOs;
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
    private readonly IGenericRepository<Topic, int> _topicRepository;
    private readonly IConfiguration _configuration;

    private const int RequiredWidth = 1200;
    private const int RequiredHeight = 1800;

    public FrameService(
        IGenericRepository<Frame, int> repository,
        IGenericRepository<Branch, int> branchRepository,
        IGenericRepository<Topic, int> topicRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IConfiguration configuration
    ) : base(repository, mapper, unitOfWork)
    {
        _branchRepository = branchRepository;
        _topicRepository = topicRepository;
        _configuration = configuration;
    }

    public override async Task<ServiceResult<FrameDto>> CreateAsync(CreateFrameDto dto)
    {
        if (dto.BranchCode != null)
        {
            try
            {
                _branchRepository.GetSingleByCondition(b => b.BranchCode == dto.BranchCode);
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult<FrameDto>.NotFound($"Không tồn tại BranchCode = {dto.BranchCode}");
            }
        }

        var topics = _topicRepository.GetMulti(t => dto.TopicIds.Contains(t.TopicId));
        if (topics.Count() != dto.TopicIds.Count())
        {
            return ServiceResult<FrameDto>.NotFound($"Không tồn tại Topic với Id: {string.Join(", ", dto.TopicIds)}");
        }

        byte[] subjectBytes, backgroundBytes, overlayBytes;
        try
        {
            subjectBytes = Convert.FromBase64String(CleanBase64(dto.Subject));
            backgroundBytes = Convert.FromBase64String(CleanBase64(dto.Background));
            overlayBytes = Convert.FromBase64String(CleanBase64(dto.Overlay));
        }
        catch (FormatException)
        {
            return ServiceResult<FrameDto>.ValidationError("Dữ liệu base64 không hợp lệ");
        }

        var subjectCheck = ValidateImageDimension(subjectBytes, "Subject");
        if (subjectCheck != null) return ServiceResult<FrameDto>.ValidationError(subjectCheck);

        var backgroundCheck = ValidateImageDimension(backgroundBytes, "Background");
        if (backgroundCheck != null) return ServiceResult<FrameDto>.ValidationError(backgroundCheck);

        var overlayCheck = ValidateImageDimension(overlayBytes, "Overlay");
        if (overlayCheck != null) return ServiceResult<FrameDto>.ValidationError(overlayCheck);

        try
        {
            var frame = new Frame
            {
                BranchCode = dto.BranchCode,
                Branchname = dto.BranchName,
                FrameName = dto.FrameName,
                Subject = string.Empty,
                Background = string.Empty,
                Overlay = string.Empty,
                Base64Subject = dto.Subject,
                Base64Background = dto.Background,
                Base64Overlay = dto.Overlay,
                CreatedAt = DateTime.UtcNow,
                TopicsFrames = topics.Select(t => new TopicsFrames { TopicId = t.TopicId }).ToList()
            };

            _repository.Add(frame);
            await _unitOfWork.SaveChangesAsync();

            var subjectUrl = await SaveImageAsync(frame.FrameId, "subject", subjectBytes);
            var backgroundUrl = await SaveImageAsync(frame.FrameId, "background", backgroundBytes);
            var overlayUrl = await SaveImageAsync(frame.FrameId, "overlay", overlayBytes);

            frame.Subject = subjectUrl;
            frame.Background = backgroundUrl;
            frame.Overlay = overlayUrl;

            await _unitOfWork.SaveChangesAsync();

            var result = new FrameDto
            {
                FrameId = frame.FrameId,
                BranchCode = frame.BranchCode,
                BranchName = frame.Branchname,
                FrameName = frame.FrameName,
                Topics = topics.Select(t => new FrameTopicDto
                {
                    TopicId = t.TopicId,
                    TopicName = t.TopicName,
                    LayoutType = t.layoutType
                }).ToList(),
                Subject = frame.Subject,
                Background = frame.Background,
                Overlay = frame.Overlay,
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
            string[] includes = { "Branch", "TopicsFrames.Topic" };

            var genericParams = parameters.ToGenericQueryParameters();

            Expression<Func<Frame, bool>>? predicate = layout == LayoutType.All
                ? null
                : f => f.TopicsFrames.Any(tf => tf.Topic.layoutType == layout);

            var pagedFrames = _repository.GetPaged(predicate, genericParams, null, includes);
            var result = pagedFrames.Items.Select(f => new FrameDto
            {
                FrameId = f.FrameId,
                BranchCode = f.BranchCode,
                BranchName = f.Branch?.BranchName ?? f.Branchname ?? string.Empty,
                FrameName = f.FrameName,
                Topics = f.TopicsFrames.Select(tf => new FrameTopicDto
                {
                    TopicId = tf.TopicId,
                    TopicName = tf.Topic.TopicName,
                    LayoutType = tf.Topic.layoutType
                }).ToList(),
                Subject = f.Subject,
                Background = f.Background,
                Overlay = f.Overlay,
                Base64Subject = f.Base64Subject,
                Base64Background = f.Base64Background,
                Base64Overlay = f.Base64Overlay,
                CreatedAt = f.CreatedAt
            });

            var pagedResult = new PagedResult<FrameDto>(
                result,
                pagedFrames.TotalCount,
                pagedFrames.Index,
                pagedFrames.PageSize
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
            string[] includes = { "Branch", "TopicsFrames.Topic" };
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
                BranchCode = frame.BranchCode,
                BranchName = frame.Branch?.BranchName ?? frame.Branchname ?? string.Empty,
                FrameName = frame.FrameName,
                Topics = frame.TopicsFrames.Select(tf => new FrameTopicDto
                {
                    TopicId = tf.TopicId,
                    TopicName = tf.Topic.TopicName,
                    LayoutType = tf.Topic.layoutType
                }).ToList(),
                Subject = frame.Subject,
                Background = frame.Background,
                Overlay = frame.Overlay,
                Base64Subject = frame.Base64Subject,
                Base64Background = frame.Base64Background,
                Base64Overlay = frame.Base64Overlay,
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
            using var ms = new MemoryStream(imageBytes);
            using var image = Image.Load(ms);
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

    public ServiceResult<IEnumerable<FrameOptionDto>> GetAllOptions()
    {
        var frames = _repository.GetAll();
        var result = frames.Select(f => new FrameOptionDto
        {
            FrameId = f.FrameId,
            FrameName = f.FrameName,
        });
        return ServiceResult<IEnumerable<FrameOptionDto>>.Success(result);
    }
}
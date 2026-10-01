using System.Linq.Expressions;
using Application.DTOs;
using Application.DTOs.Commons;
using Application.Interfaces;
using Application.Interfaces.Commons;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Shared.QueryParameter;
using Shared.Results;
using SixLabors.ImageSharp;


namespace Application.Services;
public class TopicService : GenericService<Topic, TopicDto, CreateTopicDto, int>, ITopicService
{
    private readonly IGenericRepository<Frame, int> _frameRepository;
    private readonly IConfiguration _configuration;
    public TopicService(
        IGenericRepository<Topic, int> repository,
        IGenericRepository<Frame, int> frameRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IConfiguration configuration
    ) : base(repository,  mapper, unitOfWork)
    {
        _frameRepository = frameRepository;
        _configuration = configuration;    
    }

    private const int RequiredWidth = 1200;
    private const int RequiredHeight = 1800;

    public async override Task<ServiceResult<TopicDto>> CreateAsync(CreateTopicDto dto)
    {
        try
        {
            var topic = _repository.GetSingleByCondition(t => t.TopicName == dto.TopicName);
            return ServiceResult<TopicDto>.InternalServerError($"Đã tồn tại topic: {dto.TopicName}");
        }
        catch (KeyNotFoundException)
        {
            var frame = _frameRepository.GetMulti(f => dto.FrameIds.Contains(f.FrameId));
            if(frame.Count() != dto.FrameIds.Count())
            {
                return ServiceResult<TopicDto>.NotFound($"Không tồn tại Frame với Id: {string.Join(", ", dto.FrameIds)}");
            }
            byte[] avatarBytes;
            try
            {
                avatarBytes = Convert.FromBase64String(CleanBase64(dto.Avatar));
            }
            catch (FormatException)
            {
                return ServiceResult<TopicDto>.ValidationError("Dữ liệu base64 không hợp lệ");
            }
            var avatarCheck = ValidateImageDimension(avatarBytes, "Avatar");
            if(avatarCheck != null) return ServiceResult<TopicDto>.ValidationError(avatarCheck);

            var topic = new Topic
            {
                TopicName = dto.TopicName,
                Avatar = string.Empty,
                layoutType = dto.layoutType,
                BranchCode = dto.BranchCode,
                CreateAt = DateTime.UtcNow
            };
            _repository.Add(topic);
            await _unitOfWork.SaveChangesAsync();

            var avatarUrl = await SaveImageAsync(topic.TopicId, "avatar", avatarBytes);
            topic.Avatar = avatarUrl;
            await _unitOfWork.SaveChangesAsync();
            
            var result = new TopicDto
            {
                TopicId = topic.TopicId,
                TopicName = topic.TopicName,
                layoutType = topic.layoutType,
                FrameCount = 0,
                BranchCode = topic.BranchCode,
                CreateAt = topic.CreateAt
            };

            return ServiceResult<TopicDto>.Created(result);
        }
        catch(Exception ex)
        {
            return ServiceResult<TopicDto>.InternalServerError($"Lỗi tạo topic: {ex.Message}");
        }

    }

    public ServiceResult<PagedResult<TopicDto>> GetAll(TopicQueryParameters parameters)
    {
        try
        {
            var genericParams = new GenericQueryParameters
            {
                Take = parameters.Take,
                Index = parameters.Index,
                PageSize = parameters.PageSize,
                SortBy = parameters.SortBy,
                SortDirection = parameters.SortDirection,
                Search = parameters.Search         
            };
            Expression<Func<Topic, bool>>? predicate = null;
            if (parameters.layoutType.HasValue && !string.IsNullOrEmpty(parameters.BranchCode))
                predicate = t => t.layoutType == parameters.layoutType.Value && t.BranchCode == parameters.BranchCode;
            else if (parameters.layoutType.HasValue)
                predicate = t => t.layoutType == parameters.layoutType.Value;
            else if (!string.IsNullOrEmpty(parameters.BranchCode))
                predicate = t => t.BranchCode == parameters.BranchCode;

            string[] searchProperties = { "TopicName", "BranchCode", "layoutType" };
            string[] includes = { "TopicsFrames.Frame" };
            var pagedEntities = _repository.GetPaged(predicate, genericParams, searchProperties, includes);

            var result = pagedEntities.Items.Select(_mapper.Map<Topic, TopicDto>);

            var pagedResult = new PagedResult<TopicDto>(
                result,
                pagedEntities.TotalCount,
                pagedEntities.Index,
                pagedEntities.PageSize
            );
            return ServiceResult<PagedResult<TopicDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return ServiceResult<PagedResult<TopicDto>>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
        }
    }

    public ServiceResult<IEnumerable<TopicOptionDto>> GetAllOptions()
    {
        var topics = _repository.GetAll();
        var result = topics.Select(t => new TopicOptionDto
        {
            TopicName = t.TopicName,
            TopicId = t.TopicId,
        });
        return ServiceResult<IEnumerable<TopicOptionDto>>.Success(result);
    }
    
    public override ServiceResult<TopicDto> GetById(int id)
    {
        Topic topic;
        try
        {
            topic = _repository.GetSingleById(id);
            var result = _mapper.Map<TopicDto>(topic);
            return ServiceResult<TopicDto>.Success(result);
        }
        catch (KeyNotFoundException)
        {
            return ServiceResult<TopicDto>.NotFound($"Không tìm thấy TopicId = {id}");
        }
        catch(Exception ex)
        {
            return ServiceResult<TopicDto>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
        }
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

    private static string CleanBase64(string base64)
    {
        var commaIndex = base64.IndexOf(',');
        return commaIndex >= 0 ? base64[(commaIndex + 1)..] : base64;
    }

    private async Task<string> SaveImageAsync(int topicId, string imageType, byte[] imageBytes)
    {
        var storagePath = _configuration["TopicStorage:Path"]
            ?? throw new InvalidOperationException("Thiếu cấu hình TopicStorage:Path");
        var baseUrl = _configuration["TopicStorage:BaseUrl"]
            ?? throw new InvalidOperationException("Thiếu cấu hình TopicStorage:BaseUrl");

        var rootPath = Path.IsPathRooted(storagePath)
            ? storagePath
            : Path.Combine(Directory.GetCurrentDirectory(), storagePath);

        var folderPath = Path.Combine(rootPath, topicId.ToString());

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileName = $"{imageType}.png";
        var filePath = Path.Combine(folderPath, fileName);

        await File.WriteAllBytesAsync(filePath, imageBytes);

        return $"{baseUrl}/Topic/{topicId}/{fileName}";
    }
}
using Application.DTOs.Identites;
using Application.Interfaces;
using Application.Interfaces.Commons;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Shared.Results;

namespace Application.Services;

public class VideoService : GenericService<Video, VideoDto, CreateVideoDto, int>, IVideoService
{
    private readonly IGenericRepository<Invoice, int> _invoiceRepository;
    private readonly string _videoRoot;

    public VideoService(
        IGenericRepository<Video, int> repository,
        IGenericRepository<Invoice, int> invoiceRepository,
        IConfiguration configuration,
        IMapper mapper,
        IUnitOfWork unitOfWork
    ) : base(repository, mapper, unitOfWork)
    {
        _invoiceRepository = invoiceRepository;

        var path = configuration["VideoStorage:Path"]
            ?? throw new InvalidOperationException("Thiếu cấu hình VideoStorage:Path");
        _videoRoot = Path.GetFullPath(path, AppContext.BaseDirectory);
    }

    public async Task<ServiceResult<VideoDto>> UploadAsync(CreateVideoDto dto, Stream content)
    {
        Invoice? invoice;
        try
        {
            invoice = _invoiceRepository.GetSingleByCondition(i => i.InvoiceCode == dto.InvoiceCode);
            if (invoice == null)
            {
                return ServiceResult<VideoDto>.ValidationError("Không tồn tại hóa đơn");
            }
            if (invoice.BoothId != dto.BoothId)
            {
                return ServiceResult<VideoDto>.ValidationError("Hóa đơn không thuộc booth này");
            }
            if (_repository.CheckContains(v => v.InvoiceCode == dto.InvoiceCode))
            {
                return ServiceResult<VideoDto>.ValidationError("Đã tồn tại video cho hóa đơn này");
            }
        }
        catch (Exception ex)
        {
            return ServiceResult<VideoDto>.InternalServerError($"Lỗi tạo video 000: {ex.Message}");
        }

        var fileName = $"{dto.BoothId}_{dto.InvoiceCode}.mp4";
        var finalPath = Path.Combine(_videoRoot, fileName);
        var tempPath = finalPath + ".tmp";

        try
        {
            Directory.CreateDirectory(_videoRoot);
            await using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await content.CopyToAsync(fs);
            }
            File.Move(tempPath, finalPath);
        }
        catch (Exception ex)
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
            return ServiceResult<VideoDto>.InternalServerError($"Lỗi tạo video 001: {ex.Message}");
        }

        try
        {
            var video = new Video
            {
                BoothId = dto.BoothId,
                InvoiceCode = dto.InvoiceCode,
                VideoPath = fileName,
                CreatedAt = DateTime.UtcNow
            };
            _repository.Add(video);
            await _unitOfWork.SaveChangesAsync();

            var result = new VideoDto
            {
                BoothId = video.BoothId,
                InvoiceCode = video.InvoiceCode,
                VideoPath = video.VideoPath,
                CreatedAt = video.CreatedAt
            };
            return ServiceResult<VideoDto>.Created(result);
        }
        catch (Exception ex)
        {
            if (File.Exists(finalPath)) File.Delete(finalPath);
            return ServiceResult<VideoDto>.InternalServerError($"Lỗi tạo video 002: {ex.Message}");
        }
    }    
}
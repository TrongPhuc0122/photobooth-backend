using Application.Interfaces;
using Application.DTOs.Identites;
using Application.Interfaces.Commons;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Shared.Results;
using QRCoder;

namespace Application.Services;
public class PhotoService : GenericService<Photo, PhotoDto, CreatePhotoDto, int>, IPhotoServices 
{
    private readonly IGenericRepository<Booths, Guid> _boothRepository;
    private readonly string _imagePath;
    private readonly string _baseUrl;
    public PhotoService(
        IGenericRepository<Booths, Guid> boothRepository,
        IGenericRepository<Photo, int> repository,
        IConfiguration configuration,
        IMapper mapper,
        IUnitOfWork unitOfWork
    ) : base(repository, mapper, unitOfWork)
    {
        _boothRepository = boothRepository;
        _imagePath = configuration["ImageStorage:Path"]!;
        _baseUrl = configuration["ImageStorage:BaseUrl"]!;

    }
    public async Task<ServiceResult<PhotoDto>> CreatePhoto(CreatePhotoDto dto)
    {
        if (!_boothRepository.CheckContains(b => b.BoothId == dto.BoothId))
        return ServiceResult<PhotoDto>.NotFound($"Không tìm thấy booth {dto.BoothId}");

        byte[] imageBytes = Convert.FromBase64String(dto.Base64Image);
        var photo = new Photo
        {
            BoothId = dto.BoothId,
            ImageUrl = string.Empty,
            PrintCount = 0,
            CreatedAt = DateTime.UtcNow
        };
        _repository.Add(photo);
        await _unitOfWork.SaveChangesAsync();

        var filePath = Path.Combine(_imagePath, $"{photo.PhotoId}.png");
        await File.WriteAllBytesAsync(filePath, imageBytes);
        photo.ImageUrl = $"images/{photo.PhotoId}.png";
        _repository.Update(photo);
        await _unitOfWork.SaveChangesAsync();

        var imageUrl = $"{_baseUrl}/images/{photo.PhotoId}.png";
        var qrGenerator = new QRCodeGenerator();
        var qrData = qrGenerator.CreateQrCode(imageUrl, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrData);
        byte[] qrBytes = qrCode.GetGraphic(10);
        var qrBase64 = Convert.ToBase64String(qrBytes);


        return ServiceResult<PhotoDto>.Created(new PhotoDto
        {
            QRCode = qrBase64,
            URL = imageUrl
        });
    }
}

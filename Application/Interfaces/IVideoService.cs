using Application.DTOs.Identites;
using Application.Interfaces.Commons;
using Domain.Entities;
using Shared.Results;

namespace Application.Interfaces;
public interface IVideoService : IGenericService<Video, VideoDto, CreateVideoDto, int>
{
    Task<ServiceResult<VideoDto>> UploadAsync(CreateVideoDto dto, Stream content);
}
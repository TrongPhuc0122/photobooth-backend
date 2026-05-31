using Application.DTOs.Identites.Booths;
using Application.Interfaces.Commons;
using Application.DTOs.Commons;
using Domain.Entities;
using Shared.Results;
using Application.DTOs.Identites;

namespace Application.Interfaces;
public interface IPhotoServices : IGenericService<Photo, PhotoDto, CreatePhotoDto, int>
{
    Task<ServiceResult<PhotoDto>> CreatePhoto(CreatePhotoDto dto);
}
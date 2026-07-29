using Application.DTOs;
using Application.DTOs.Commons;
using Application.DTOs.Identites;
using Application.Interfaces.Commons;
using Domain.Entities;
using Shared;
using Shared.Results;

namespace Application.Interfaces
{
    public interface IFrameService : IGenericService<Frame, FrameDto, CreateFrameDto, int>
    {
        ServiceResult<PagedResult<FrameDto>> GetAll(CommonQueryParameters parameters, LayoutType layout);
    }
}
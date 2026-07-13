using Application.DTOs.Commons;
using Application.Interfaces.Commons;
using Application.DTOs.Identites;
using Domain.Entities;
using Shared.Results;

namespace Application.Interfaces
{
    public interface IFrameService : IGenericService<Frame, FrameDto, CreateFrameDto, int>
    {
        Task<ServiceResult<FrameDto>> CreateFrame(CreateFrameDto dto);
        ServiceResult<PagedResult<FrameDto>> GetAll(FrameQueryParameters parameters);
        Task<ServiceResult<FrameDto>> UpdateFrame(int frameId, UpdateFrameDto dto);
        Task<ServiceResult> SoftDeleteFrame(int frameId);
    }
}


using Application.DTOs.Identites.Booths;
using Application.Interfaces.Commons;
using Application.DTOs.Commons;
using Domain.Entities;
using Shared.Results;

namespace Application.Interfaces
{
    public interface IBoothService : IGenericService<Booths, BoothDto, CreateBoothDto, Guid>
    {
        ServiceResult<PagedResult<BoothDto>> GetAll(CommonQueryParameters parameters);
        Task<ServiceResult> CreateError(Guid boothId, CreateBoothErrorDto dto);
        ServiceResult<IEnumerable<BoothErrorDto>> GetActiveErrors(Guid boothId);
        ServiceResult<IEnumerable<BoothErrorDto>> GetAllErrors(Guid boothId);
        Task<ServiceResult> FixError(Guid boothId, string errorCode);
        Task<ServiceResult> UpdateResources (Guid boothId, int? paper, int? ribbon);
        Task<ServiceResult> SetBoothStorage (Guid boothId, int? paperMax, int? ribbonMax);
        Task<ServiceResult> GetHeartBeat (Guid boothId);
    }
}

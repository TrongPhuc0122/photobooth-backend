using Application.DTOs.Identites.Booths;
using Application.Interfaces.Commons;
using Domain.Entities;
using Shared.Results;

namespace Application.Interfaces
{
    public interface IBoothService : IGenericService<Booths, BoothDto, CreateBoothDto, int>
    {
        Task<ServiceResult> CreateError(int boothId, CreateBoothErrorDto dto);
        ServiceResult<IEnumerable<BoothErrorDto>> GetActiveErrors(int boothId);
        ServiceResult<IEnumerable<BoothErrorDto>> GetAllErrors(int boothId);
        Task<ServiceResult> FixError(int boothId, string errorCode);
        Task<ServiceResult> UpdateResources (int boothId, int? paper, int? ribbon);
        Task<ServiceResult> SetBoothStorage (int boothId, int? paperMax, int? ribbonMax);
    }
}

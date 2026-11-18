using Application.DTOs.Identites.Payment;
using Shared.Results;

namespace Application.Interfaces;

public interface IEPAYClientService
{
    Task<ServiceResult<RegisterResDto>> RegisterAsync(CreateRegisterReqDto dto);

}
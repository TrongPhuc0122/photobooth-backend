using Application.Interfaces.Commons;
using Shared.Results;

namespace Application.DTOs.Identites;
public interface ISummeryService
{
    ServiceResult<SummeryDto> GetSummeryInfor(); 
}
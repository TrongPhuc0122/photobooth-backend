using Application.Interfaces.Commons;
using Application.DTOs.Identites;
using Application.DTOs.Commons;
using Domain.Entities;
using Shared.Results;

namespace Application.Interfaces
{
    public interface IBranchService : IGenericService<Branch, BranchDto, CreateBranchDto, int>
    {
       ServiceResult<PagedResult<BranchDto>> GetAll(BranchQueryParameters Parameters);
       ServiceResult<IEnumerable<BranchOptionDto>> GetAllOptions();
    }
}

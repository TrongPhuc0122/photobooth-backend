    using Application.Interfaces.Commons;
    using Application.DTOs.Identites;
    using Domain.Entities;
    using Shared.Results;

    namespace Application.Interfaces
    {
        public interface IBranchService : IGenericService<Branch, BranchDto, CreateBranchDto, int>
        {
            
        }
    }

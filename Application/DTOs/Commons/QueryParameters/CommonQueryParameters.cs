using Shared.QueryParameter;

namespace Application.DTOs.Commons;
public class CommonQueryParameters : BaseQueryParameters
{
    public int? BranchId { get; set; }
        public virtual GenericQueryParameters ToGenericQueryParameters()
        {
            var genericParams = new GenericQueryParameters
            {
                Take = Take,
                Index = Index,
                PageSize = PageSize,
                SortBy = SortBy,
                SortDirection = SortDirection,
                Search = Search 
            };
            if (BranchId.HasValue)
            {
                genericParams.AddFilter("BranchId", "==", BranchId.Value);
            }
            return genericParams;
        }
}
using Shared;
using Shared.QueryParameter;

namespace Application.DTOs.Commons;
public class BranchQueryParameters : BaseQueryParameters
{
    public Status? Status { get; set; }
    public Guid? BoothId{ get; set; }
}
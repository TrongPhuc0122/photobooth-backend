using Shared;
using Shared.QueryParameter;

namespace Application.DTOs.Commons;
public class BoothQueryParameters : BaseQueryParameters
{
    public Status? Status { get; set; }
    public int? BranchId { get; set; }
}
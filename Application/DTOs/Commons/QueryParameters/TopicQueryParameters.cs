using Shared;
using Shared.QueryParameter;

namespace Application.DTOs.Commons;

public class TopicQueryParameters : BaseQueryParameters
{
    public LayoutType? layoutType { get; set; }
    public string? BranchCode { get; set; }
}
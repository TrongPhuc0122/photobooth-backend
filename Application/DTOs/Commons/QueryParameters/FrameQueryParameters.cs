using Shared.QueryParameter;

namespace Application.DTOs.Commons;

public class FrameQueryParameters : CommonQueryParameters
{
    public string? BranchName { get; set; }

    public override GenericQueryParameters ToGenericQueryParameters()
    {
        var genericParams = base.ToGenericQueryParameters();

        if (!string.IsNullOrWhiteSpace(BranchName))
        {
            genericParams.AddFilter("Branchname", "contains", BranchName.Trim());
        }

        return genericParams;
    }
}

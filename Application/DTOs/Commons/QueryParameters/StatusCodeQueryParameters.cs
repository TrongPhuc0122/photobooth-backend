using Shared;

namespace Application.DTOs.Commons;

public class StatusCodeQueryParameters : CommonQueryParameters
{
    public StatusType? Type { get; set; }
}
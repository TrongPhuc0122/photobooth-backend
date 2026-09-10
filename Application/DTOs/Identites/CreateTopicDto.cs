using Shared;

namespace Application.DTOs;
public class CreateTopicDto
{
    public required string TopicName { get; set; } = string.Empty;
    public LayoutType layoutType { get; set; }
    public string? BranchCode { get; set; }
}
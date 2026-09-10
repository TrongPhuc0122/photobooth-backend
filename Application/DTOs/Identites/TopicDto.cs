using Shared;

namespace Application.DTOs;
public class TopicDto
{
    public int TopicId { get; set; }
    public required string TopicName { get; set; } = string.Empty;
    public LayoutType layoutType { get; set; }
    public int FrameCount { get; set; }
    public string? BranchCode { get; set; }
    public DateTime CreateAt { get; set; }
}
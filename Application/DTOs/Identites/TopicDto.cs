using Shared;

namespace Application.DTOs;
public class TopicDto
{
    public int TopicId { get; set; }
    public required string TopicName { get; set; } = string.Empty;
    public LayoutType layoutType { get; set; }
    public string Base64Avatar { get; set; } = string.Empty;
    public int FrameCount { get; set; }
    public string Avatar { get; set; } = string.Empty;
    public List<TopicFrame> Frames { get; set; } = new();
    public string? BranchCode { get; set; }
    public DateTime CreateAt { get; set; }
}

public class TopicFrame{
    public string FrameName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Background { get; set; } = string.Empty;
    public string Overlay { get; set; } = string.Empty;
}
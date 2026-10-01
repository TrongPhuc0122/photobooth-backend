using Shared;

namespace Application.DTOs.Identites;

public class FrameTopicDto
{
    public int TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public LayoutType LayoutType { get; set; }
}

public class FrameDto
{
    public int FrameId { get; set; }
    public string? BranchCode { get; set; }
    public string? BranchName { get; set; } = string.Empty;
    public string FrameName { get; set; } = string.Empty;
    public List<FrameTopicDto> Topics { get; set; } = new();
    public string Subject { get; set; } = string.Empty;
    public string Background { get; set; } = string.Empty;
    public string Overlay { get; set; } = string.Empty;
    public string Base64Subject { get; set; } = string.Empty;
    public string Base64Background { get; set; } = string.Empty;
    public string Base64Overlay { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
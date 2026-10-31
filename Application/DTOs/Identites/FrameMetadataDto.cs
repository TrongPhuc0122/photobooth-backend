namespace Application.DTOs.Identites;

public class FrameMetadataDto
{
    public int FrameId { get; set; }
    public string? BranchCode { get; set; }
    public string? BranchName { get; set; } = string.Empty;
    public string FrameName { get; set; } = string.Empty;
    public List<FrameTopicDto> Topics { get; set; } = new();
    public DateTime UpdateAt { get; set; }
}
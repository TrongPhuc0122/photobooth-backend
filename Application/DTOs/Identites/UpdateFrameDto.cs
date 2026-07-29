namespace Application.DTOs.Identites;

public class UpdateFrameDto
{
    public int? BranchId { get; set; }
    public string? BranchName { get; set; } = string.Empty;
    public string FrameName { get; set; } = string.Empty;
    public int FrameId { get; set; }
    public int? TopicId { get; set; }
    public string? TopicName { get; set; } = string.Empty;
    public string? SubjectImage { get; set; }
    public string? Background { get; set; }
    public string? Overlay { get; set; }
}
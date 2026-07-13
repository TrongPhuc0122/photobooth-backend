using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Identites;

public class UpdateFrameDto
{
    public int? BranchId { get; set; }
    public string? BranchName { get; set; } = string.Empty;
    public string FrameName { get; set; } = string.Empty;
    public int FrameId { get; set; }
    public int? TopicId { get; set; }
    public string? TopicName { get; set; } = string.Empty;
    public IFormFile? SubjectImage { get; set; }
    public IFormFile? Background { get; set; }
    public IFormFile? Overlay { get; set; }
}
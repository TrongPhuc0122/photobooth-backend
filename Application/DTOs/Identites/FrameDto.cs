using Microsoft.AspNetCore.Http;
using Shared;
namespace Application.DTOs.Identites;

public class FrameDto
{
    public string? BranchCode { get; set; }
    public string? BranchName { get; set; } = string.Empty;
    public  string FrameName { get; set; } = string.Empty;
    public int FrameId { get; set; }
    public int TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public LayoutType LayoutType { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Background { get; set; } = string.Empty;
    public string Overlay { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }   
}

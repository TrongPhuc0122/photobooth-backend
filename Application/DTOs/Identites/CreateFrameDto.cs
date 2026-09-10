using Microsoft.AspNetCore.Http;
using Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identites;

public class CreateFrameDto
{
    public string? BranchCode { get; set; }
    public string? BranchName { get; set; } = string.Empty;
    [Required]
    public string FrameName { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public string Subject { get; set; } = null!;
    [Required]
    public string Background { get; set; } = null!;
    [Required]
    public string Overlay { get; set; } = null!;
}
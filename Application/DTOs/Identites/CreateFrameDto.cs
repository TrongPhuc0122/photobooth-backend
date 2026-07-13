using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identites;

public class CreateFrameDto
{
    public int? BranchId { get; set; }
    public string? BranchName { get; set; } = string.Empty;
    [Required]
    public string FrameName { get; set; } = string.Empty;
    public int? TopicId { get; set; }
    public string? TopicName { get; set; } = string.Empty;
    [Required]
    public IFormFile SubjectImage { get; set; } = null!;
    [Required]
    public IFormFile Background { get; set; } = null!;
    [Required]
    public IFormFile Overlay { get; set; } = null!;
}
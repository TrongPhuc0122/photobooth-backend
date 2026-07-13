using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Commons;
using Microsoft.AspNetCore.Http;

namespace Domain.Entities;

public class Frame : BaseEntity
{
    [Key]
    public int FrameId { get; set; }
    public int? BranchId { get; set; }
    public string? Branchname { get; set; } = string.Empty;
    public string FrameName { get; set; } = string.Empty;

    public int? TopicId { get; set; }
    public string? TopicName { get; set; } = string.Empty;

    public string SubjectImageUrl { get; set; } = string.Empty;
    public string BackgroundUrl { get; set; } = string.Empty;
    public string OverlayUrl { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }
}
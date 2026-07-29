using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Commons;
using Shared;

namespace Domain.Entities;

public class Frame : BaseEntity
{
    [Key]
    public int FrameId { get; set; }
    public int? BranchId { get; set; }
    public string? Branchname { get; set; } = string.Empty;
    public required string FrameName { get; set; } = string.Empty;

    public int TopicId { get; set; }
    public required LayoutType LayoutType { get; set; }

    public required string SubjectImageUrl { get; set; } = string.Empty;
    public required string BackgroundUrl { get; set; } = string.Empty;
    public required string OverlayUrl { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }
    [ForeignKey("TopicId")]
    public virtual Topic? Topic { get; set; }
}
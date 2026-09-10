using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Commons;
using Shared;

namespace Domain.Entities;

public class Frame : BaseEntity
{
    [Key]
    public int FrameId { get; set; }
    public string? BranchCode { get; set; }
    public string? Branchname { get; set; } = string.Empty;
    public required string FrameName { get; set; } = string.Empty;
    public int TopicId { get; set; }

    public required string Subject { get; set; } = string.Empty;
    public required string Background { get; set; } = string.Empty;
    public required string Overlay { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }
    [ForeignKey("TopicId")]
    public virtual Topic? Topic { get; set; }
}
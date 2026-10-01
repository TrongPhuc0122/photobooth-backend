using Domain.Entities.Commons;
using Shared;

namespace Domain.Entities;
public class Topic : BaseEntity
{
    public int TopicId { get; set; }
    public required string TopicName { get; set; } = string.Empty;
    public required string Avatar { get; set; } = string.Empty;
    public LayoutType layoutType { get; set; }
    public string? BranchCode { get; set; }
    public DateTime CreateAt { get; set; }

    public ICollection<TopicsFrames> TopicsFrames { get; set; } = new List<TopicsFrames>();
}
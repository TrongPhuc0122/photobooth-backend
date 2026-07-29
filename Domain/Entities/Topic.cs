namespace Domain.Entities;
public class Topic
{
    public int TopicId { get; set; }
    public required string TopicName { get; set; } = string.Empty;
}
namespace Application.DTOs;
public class TopicDto
{
    public int TopicId { get; set; }
    public required string TopicName { get; set; } = string.Empty;
}
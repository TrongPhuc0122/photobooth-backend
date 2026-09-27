namespace Domain.Entities;

public class TopicsFrames
{
    public int TopicId { get; set; }
    public Topic Topic { get; set; } = null!;

    public int FrameId { get; set; }
    public Frame Frame { get; set; } = null!;
}
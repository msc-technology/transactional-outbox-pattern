namespace MeetupDemo.Shared.Outbox;

public class OutboxMessage
{
    public string Id { get; set; }
    public string MessageBody { get; set; }
    public string Exchange { get; set; }
    public string RoutingKey { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

namespace MeetupDemo.Shared.Publishers;

public interface IMessagePublisher
{
    Task PublishAsync(string exchange, string routingKey, string messageBody, string messageId);
}
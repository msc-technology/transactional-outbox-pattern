namespace MeetupDemo.Shared;

public interface IMessageHandler<in TMessage> where TMessage : class, IMessage
{
    Task HandleAsync(TMessage message, CancellationToken cancellation = default);
}
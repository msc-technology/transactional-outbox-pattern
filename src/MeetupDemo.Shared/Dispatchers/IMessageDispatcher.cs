namespace MeetupDemo.Shared.Dispatchers;

public interface IMessageDispatcher
{
    Task DispatchAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class, IMessage;
}
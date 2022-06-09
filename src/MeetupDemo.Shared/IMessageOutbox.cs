namespace MeetupDemo.Shared;

public interface IMessageOutbox
{
    bool Enabled { get; }
    Task HandleAsync(Func<Task> handler);
    Task SendAsync<TMessage>(string exchange, string routingKey, TMessage message)
        where TMessage : class;
}

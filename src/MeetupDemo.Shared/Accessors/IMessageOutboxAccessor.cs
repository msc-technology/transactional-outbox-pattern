using MeetupDemo.Shared.Outbox;

namespace MeetupDemo.Shared.Accessors;

public interface IMessageOutboxAccessor
{
    Task<IReadOnlyList<OutboxMessage>> GetUnsentAsync();
    Task ProcessAsync(OutboxMessage message);
}

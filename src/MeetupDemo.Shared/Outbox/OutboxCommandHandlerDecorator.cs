using MeetupDemo.Shared.Accessors;
using MeetupDemo.Shared.Options;

namespace MeetupDemo.Shared.Outbox;

internal sealed class OutboxCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : class, ICommand
{

    private readonly ICommandHandler<TCommand> _handler;
    private readonly IMessageIdAccessor _messageIdAccessor;
    private readonly IMessageOutbox _outbox;
    private readonly bool _enabled;

    public OutboxCommandHandlerDecorator(
        ICommandHandler<TCommand> handler,
        IMessageOutbox outbox,
        IMessageIdAccessor messageIdAccessor,
        OutboxOptions outboxOptions)
    {
        _handler = handler;
        _outbox = outbox;
        _messageIdAccessor = messageIdAccessor;
        _enabled = outboxOptions.Enabled;
    }

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        _messageIdAccessor.SetMessageId(Guid.NewGuid().ToString("N"));

        if (_enabled)
            await _outbox.HandleAsync(async () => await _handler.HandleAsync(command));
        else
            await _handler.HandleAsync(command, cancellationToken);
    }
}

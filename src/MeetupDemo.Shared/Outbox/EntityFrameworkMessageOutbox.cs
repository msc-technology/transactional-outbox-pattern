using System.Text.Json;
using System.Text.Json.Serialization;
using MeetupDemo.Shared.Accessors;
using MeetupDemo.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MeetupDemo.Shared.Outbox;

internal sealed class EntityFrameworkMessageOutbox<TContext> : IMessageOutbox, IMessageOutboxAccessor
    where TContext : DbContext
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private readonly TContext _dbContext;
    private readonly IMessageIdAccessor _messageIdAccessor;
    private readonly ILogger<EntityFrameworkMessageOutbox<TContext>> _logger;

    public bool Enabled { get; }

    public EntityFrameworkMessageOutbox(TContext dbContext, OutboxOptions options, IMessageIdAccessor messageIdAccessor,
        ILogger<EntityFrameworkMessageOutbox<TContext>> logger)
    {
        _dbContext = dbContext;
        _messageIdAccessor = messageIdAccessor;
        Enabled = options.Enabled;
        _logger = logger;
    }

    public async Task HandleAsync(Func<Task> handler)
    {
        if (!Enabled)
        {
            _logger.LogWarning("Outbox is disabled, incoming messages won't be processed.");
            return;
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation($"Processing a message...");

            await handler();

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation($"Processed a message.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"There was an error when processing a message.");
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }
    public async Task SendAsync<T>(string exchange, string routingKey, T message) where T : class
    {
        if (!Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved into the storage.");
            return;
        }

        var messageId = _messageIdAccessor.GetMessageId();
        var outboxMessagesSet = _dbContext.Set<OutboxMessage>();
        var outboxMessage = new OutboxMessage
        {
            Id = string.IsNullOrWhiteSpace(messageId) ? Guid.NewGuid().ToString("N") : messageId,
            MessageBody = JsonSerializer.Serialize(message),
            Exchange = exchange,
            RoutingKey = routingKey,
            SentAt = DateTime.UtcNow
        };
        await outboxMessagesSet.AddAsync(outboxMessage);
        await _dbContext.SaveChangesAsync();
    }

    async Task<IReadOnlyList<OutboxMessage>> IMessageOutboxAccessor.GetUnsentAsync()
    {
        var outboxMessagesSet = _dbContext.Set<OutboxMessage>();
        var outboxMessages = await outboxMessagesSet.Where(om => om.ProcessedAt == null).ToListAsync();
        return outboxMessages;
    }

    Task IMessageOutboxAccessor.ProcessAsync(OutboxMessage message)
    {
        UpdateMessage(_dbContext.Set<OutboxMessage>(), message);

        return _dbContext.SaveChangesAsync();
    }

    private static void UpdateMessage(DbSet<OutboxMessage> set, OutboxMessage message)
    {
        message.ProcessedAt = DateTime.UtcNow;
        set.Update(message);
    }
}

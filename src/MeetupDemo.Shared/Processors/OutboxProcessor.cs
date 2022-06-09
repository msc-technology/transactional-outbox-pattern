using System.Diagnostics;
using MeetupDemo.Shared.Accessors;
using MeetupDemo.Shared.Options;
using MeetupDemo.Shared.Publishers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MeetupDemo.Shared.Processors;

public class OutboxProcessor : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessagePublisher _publisher;
    private readonly OutboxOptions _options;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly TimeSpan _interval;
    private Timer _timer;

    public OutboxProcessor(IServiceProvider serviceProvider, IMessagePublisher publisher, OutboxOptions options,
        ILogger<OutboxProcessor> logger)
    {
        if (options.Enabled && options.IntervalMilliseconds <= 0)
        {
            throw new Exception($"Invalid outbox interval: {options.IntervalMilliseconds} ms.");
        }

        _serviceProvider = serviceProvider;
        _publisher = publisher;
        _options = options;
        _logger = logger;
        _interval = TimeSpan.FromMilliseconds(options.IntervalMilliseconds);

        if (options.Enabled)
        {
            _logger.LogInformation($"Outbox is enabled, message processing every {options.IntervalMilliseconds} ms.");
            return;
        }

        _logger.LogInformation("Outbox is disabled.");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return Task.CompletedTask;
        }

        _timer = new Timer(SendOutboxMessages, null, TimeSpan.Zero, _interval);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return Task.CompletedTask;
        }

        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private void SendOutboxMessages(object state)
    {
        _ = SendOutboxMessagesAsync();
    }

    private async Task SendOutboxMessagesAsync()
    {
        var jobId = Guid.NewGuid().ToString("N");
        _logger.LogInformation($"Started processing outbox messages... [job id: '{jobId}']");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        using var scope = _serviceProvider.CreateScope();
        var outbox = scope.ServiceProvider.GetRequiredService<IMessageOutboxAccessor>();
        var messages = await outbox.GetUnsentAsync();
        _logger.LogInformation($"Found {messages.Count} unsent messages in outbox [job ID: '{jobId}'].");
        if (!messages.Any())
        {
            _logger.LogInformation($"No messages to be processed in outbox [job ID: '{jobId}'].");
            return;
        }

        foreach (var message in messages.OrderBy(m => m.SentAt))
        {
            await _publisher.PublishAsync(message.Exchange, message.RoutingKey, message.MessageBody, message.Id);

            await outbox.ProcessAsync(message);
        }

        stopwatch.Stop();
        _logger.LogInformation($"Processed {messages.Count} outbox messages in {stopwatch.ElapsedMilliseconds} ms [job ID: '{jobId}'].");
    }
}

using MeetupDemo.Carts.Messages;
using MeetupDemo.Shared.Dispatchers;
using MeetupDemo.Shared.Subscribers;

namespace MeetupDemo.Carts.Services;

public class MessagingBackgroundService : BackgroundService
{
    private readonly IMessageSubscriber _messageSubscriber;
    private readonly IMessageDispatcher _dispatcher;
    private readonly ILogger<MessagingBackgroundService> _logger;

    public MessagingBackgroundService(IMessageSubscriber messageSubscriber, IMessageDispatcher dispatcher,
        ILogger<MessagingBackgroundService> logger)
    {
        _messageSubscriber = messageSubscriber;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageSubscriber
            .SubscribeMessage<FundsMessage>("carts-service-euro-queue", "EUR.*", "DigitalWallet.Funds",
                async (msg, args) =>
                {
                    _logger.LogInformation(
                        $"Received EURO message for customer: {msg.CustomerId} | Funds: {msg.CurrentFunds} | RoutingKey: {args.RoutingKey}");
                    await _dispatcher.DispatchAsync(msg);
                })
            .SubscribeMessage<FundsMessage>("carts-service-dollar-queue", "USD.*", "DigitalWallet.Funds",
                async (msg, args) =>
                {
                    _logger.LogInformation(
                        $"Received DOLLAR message for customer: {msg.CustomerId} | Funds: {msg.CurrentFunds} | RoutingKey: {args.RoutingKey}");
                    await _dispatcher.DispatchAsync(msg);
                });
    }
}
using System.Text;
using MeetupDemo.Shared.Connections;
using MeetupDemo.Shared.Options;
using RabbitMQ.Client;

namespace MeetupDemo.Shared.Publishers;

internal sealed class MessagePublisher : IMessagePublisher
{
    private readonly IModel _channel;
    private readonly OutboxOptions _outboxOptions;

    public MessagePublisher(IChannelFactory channelFactory, OutboxOptions outboxOptions)
    {
        _channel = channelFactory.Create();
        _outboxOptions = outboxOptions;
    }
    public async Task PublishAsync(string exchange, string routingKey, string messageJson, string messageId)
    {
        if (_outboxOptions.ShouldThrowNetworkException)
        {
            throw new Exception("Error Network Partition: Publish message failed!");
        }

        if (string.IsNullOrEmpty(messageId))
        {
            throw new ArgumentNullException(nameof(messageId));
        }

        var body = Encoding.UTF8.GetBytes(messageJson);

        var properties = _channel.CreateBasicProperties();
        properties.MessageId = messageId;

        _channel.ExchangeDeclare(exchange, "topic", false, false);
        _channel.BasicPublish(exchange, routingKey, mandatory: true, properties, body);
        await Task.CompletedTask;
    }
}
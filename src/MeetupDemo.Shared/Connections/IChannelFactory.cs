using RabbitMQ.Client;

namespace MeetupDemo.Shared.Connections;

public interface IChannelFactory
{
    IModel Create();
}
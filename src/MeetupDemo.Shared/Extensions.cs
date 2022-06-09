using MeetupDemo.Shared.Accessors;
using MeetupDemo.Shared.Connections;
using MeetupDemo.Shared.Dispatchers;
using MeetupDemo.Shared.Options;
using MeetupDemo.Shared.Outbox;
using MeetupDemo.Shared.Processors;
using MeetupDemo.Shared.Publishers;
using MeetupDemo.Shared.Subscribers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace MeetupDemo.Shared;

public static class Extensions
{
    private const string SectionName = "outbox";

    public static IServiceCollection AddMessaging(this IServiceCollection services, string sectionName = SectionName)
    {
        var factory = new ConnectionFactory { HostName = "20.223.24.239" };
        var connection = factory.CreateConnection();

        services.AddSingleton(connection);
        services.AddSingleton<ChannelAccessor>();
        services.AddSingleton<IChannelFactory, ChannelFactory>();
        services.AddSingleton<IMessagePublisher, MessagePublisher>();
        services.AddSingleton<IMessageSubscriber, MessageSubscriber>();
        services.AddSingleton<IMessageDispatcher, MessageDispatcher>();
        services.AddSingleton<IMessageIdAccessor, MessageIdAccessor>();
        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();

        services.Scan(cfg => cfg.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(cfg => cfg.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(c => c.AssignableTo(typeof(IMessageHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var options = configuration.GetOptions<OutboxOptions>(sectionName);
        services.AddSingleton(options);

        return services;
    }

    public static IServiceCollection AddCommanding(this IServiceCollection services)
    {
        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();

        services.Scan(cfg => cfg.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
    public static IServiceCollection AddMessageOutbox<T>(this IServiceCollection services)
        where T : DbContext
    {
        services.AddTransient<IMessageOutbox, EntityFrameworkMessageOutbox<T>>();
        services.AddTransient<IMessageOutboxAccessor, EntityFrameworkMessageOutbox<T>>();
        services.TryDecorate(typeof(ICommandHandler<>), typeof(OutboxCommandHandlerDecorator<>));
        services.AddHostedService<OutboxProcessor>();

        return services;
    }
}
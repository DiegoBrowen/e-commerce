using EventBus.Interfaces;
using EventBusRabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventBus.IoC;

public static class BuildingBlokcsInjectorContainer
{
    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

            var factory = new ConnectionFactory()
            {
                //HostName = configuration.GetRequiredConnectionString("EventBus"),
                HostName = "localhost",
                DispatchConsumersAsync = true
            };

            //if (!string.IsNullOrEmpty(eventBusSection["UserName"]))
            //{
            //    factory.UserName = eventBusSection["UserName"];
            //}

            //if (!string.IsNullOrEmpty(eventBusSection["Password"]))
            //{
            //    factory.Password = eventBusSection["Password"];
            //}

            //var retryCount = eventBusSection.GetValue("RetryCount", 5);
            factory.UserName = "guest";
            factory.Password = "guest";
            var retryCount = 5;

            return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
        });

        services.AddSingleton<IEventBus, EventBusRabbitMQService>(sp =>
        {
            //var subscriptionClientName = eventBusSection.GetRequiredValue("SubscriptionClientName");
            var subscriptionClientName = "Admin";
            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQService>>();
            var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
            //var retryCount = eventBusSection.GetValue("RetryCount", 5);
            var retryCount = 5;
            return new EventBusRabbitMQService(rabbitMQPersistentConnection, logger, sp, eventBusSubscriptionsManager, subscriptionClientName, retryCount);
        });
        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
    }
}
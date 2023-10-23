using EventBusRabbitMQ;
using IntegrationEventLog;
using IntegrationEventLog.Services;
using Microsoft.EntityFrameworkCore;
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

            var factory = new ConnectionFactory
            {
                //HostName = configuration.GetRequiredConnectionString("EventBus"),
                //if (!string.IsNullOrEmpty(eventBusSection["UserName"]))
                //{
                //    factory.UserName = eventBusSection["UserName"];
                //}

                //if (!string.IsNullOrEmpty(eventBusSection["Password"]))
                //{
                //    factory.Password = eventBusSection["Password"];
                //}

                //var retryCount = eventBusSection.GetValue("RetryCount", 5);
                HostName = "localhost",
                DispatchConsumersAsync = true,
                UserName = "guest",
                Password = "guest"
            };
            var retryCount = 5;

            return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
        });

        services.AddSingleton<IEventBus, EventBusRabbitMQService>(sp =>
        {
            //var subscriptionClientName = eventBusSection.GetRequiredValue("SubscriptionClientName");
            var subscriptionClientName = "Admin";
            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQService>>();
            // var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
            //var retryCount = eventBusSection.GetValue("RetryCount", 5);
            var retryCount = 5;
            return new EventBusRabbitMQService(rabbitMQPersistentConnection, logger, sp, subscriptionClientName, retryCount);
        });
        //services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        services.AddIntegrationServices();
    }

    public static IServiceCollection AddIntegrationServices(this IServiceCollection services)
    {
        services.AddDbContext<IntegrationEventLogContext>(
            options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

        services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService>();

        return services;
    }
}
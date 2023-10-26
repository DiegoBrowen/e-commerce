using EventBus;
using EventBus.IoC;
using Home.Api.IntegrationEvents;
using Home.Api.IntegrationEvents.EventHandling;

namespace Home.Api.Providers;

public static class DependencyInjectionProvider
{
    public static void RegisterDependencies(this WebApplicationBuilder builder)
    {
        BuildingBlokcsInjectorContainer.Register(builder.Services);
        builder.Services.AddTransient<ProductCreatedIntegrationEventHandler>();
    }

    public static void UseSubscriptions(this WebApplication app)
    {
        var eventBus = app.Services.GetRequiredService<IEventBus>();
        eventBus.Subscribe<ProductCreatedIntegrationEvent, ProductCreatedIntegrationEventHandler>();
    }
}
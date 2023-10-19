using Admin.Api.IntegrationEvents.Services;
using EventBus.IoC;

namespace Admin.Api.Providers;

public static class DependencyInjectionProvider
{
    public static void RegisterDependencies(this WebApplicationBuilder builder)
    {
        BuildingBlokcsInjectorContainer.Register(builder.Services);
        builder.Services.AddScoped<IAdminIntegrationEventService, AdminIntegrationEventService>();
    }
}
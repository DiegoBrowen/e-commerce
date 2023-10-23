namespace Admin.Api.IntegrationEvents.Services;

public interface IAdminIntegrationEventService
{
    Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt);

    Task PublishThroughEventBusAsync(IntegrationEvent evt);
}
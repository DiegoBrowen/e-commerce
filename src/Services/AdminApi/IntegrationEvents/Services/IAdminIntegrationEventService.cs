namespace Admin.Api.IntegrationEvents.Services;

public interface IAdminIntegrationEventService
{
    Task PublishThroughEventBusAsync(IntegrationEvent evt);
}
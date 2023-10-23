using Admin.Api.Infrastructures;
using EventBus;
using IntegrationEventLog;
using IntegrationEventLog.Services;

namespace Admin.Api.IntegrationEvents.Services;

public class AdminIntegrationEventService : IAdminIntegrationEventService
{
    private readonly IIntegrationEventLogService _eventLogService;
    private readonly IEventBus _eventBus;

    private readonly AdminContext _adminContext;

    private readonly ILogger<AdminIntegrationEventService> _logger;

    public AdminIntegrationEventService(
        ILogger<AdminIntegrationEventService> logger,
        IEventBus eventBus,
        AdminContext adminContext,
        IIntegrationEventLogService eventLogService
       )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _adminContext = adminContext ?? throw new ArgumentNullException(nameof(adminContext));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _eventLogService = eventLogService;
    }

    public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
    {
        try
        {
            _logger.LogInformation("Publishing integration event: {IntegrationEventId_published} - ({@IntegrationEvent})", evt.Id, evt);

            await _eventLogService.MarkEventAsInProgressAsync(evt.Id);
            _eventBus.Publish(evt);
            await _eventLogService.MarkEventAsPublishedAsync(evt.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})", evt.Id, evt);
            await _eventLogService.MarkEventAsFailedAsync(evt.Id);
        }
    }

    public async Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt)
    {
        _logger.LogInformation("CatalogIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}", evt.Id);

        await ResilientTransaction.New(_adminContext).ExecuteAsync(async () =>
        {
            await _adminContext.SaveChangesAsync();
            await _eventLogService.SaveEventAsync(evt);
        });
    }
}
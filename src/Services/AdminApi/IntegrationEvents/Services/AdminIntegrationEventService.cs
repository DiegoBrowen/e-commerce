using Admin.Api.Infrastructures;
using EventBus;

namespace Admin.Api.IntegrationEvents.Services;

public class AdminIntegrationEventService : IAdminIntegrationEventService
{
    //private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
    private readonly IEventBus _eventBus;

    private readonly AdminContext _adminContext;

    //private readonly IIntegrationEventLogService _eventLogService;
    private readonly ILogger<AdminIntegrationEventService> _logger;

    private volatile bool disposedValue;

    public AdminIntegrationEventService(
        ILogger<AdminIntegrationEventService> logger,
        IEventBus eventBus,
        AdminContext adminContext
       // Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory
       //
       )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _adminContext = adminContext ?? throw new ArgumentNullException(nameof(adminContext));
        // _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        //_eventLogService = _integrationEventLogServiceFactory(_adminContext.Database.GetDbConnection());
    }

    public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
    {
        try
        {
            _logger.LogInformation("Publishing integration event: {IntegrationEventId_published} - ({@IntegrationEvent})", evt.Id, evt);

            // await _eventLogService.MarkEventAsInProgressAsync(evt.Id);
            _eventBus.Publish(evt);
            //await _eventLogService.MarkEventAsPublishedAsync(evt.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})", evt.Id, evt);
            // await _eventLogService.MarkEventAsFailedAsync(evt.Id);
        }
    }

    //public async Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt)
    //{
    //    _logger.LogInformation("CatalogIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}", evt.Id);

    //    //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
    //    //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
    //    await ResilientTransaction.New(_adminContext).ExecuteAsync(async () =>
    //    {
    //        // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
    //        await _adminContext.SaveChangesAsync();
    //        await _eventLogService.SaveEventAsync(evt, _adminContext.Database.CurrentTransaction);
    //    });
    //}

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            //if (disposing)
            //{
            //    (_eventLogService as IDisposable)?.Dispose();
            //}

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
using EventBus.Interfaces;

namespace Home.Api.IntegrationEvents.EventHandling;

public class ProductCreatedIntegrationEventHandler : IIntegrationEventHandler<ProductCreatedIntegrationEvent>
{
    public Task Handle(ProductCreatedIntegrationEvent @event)
    {
        throw new NotImplementedException();
    }
}
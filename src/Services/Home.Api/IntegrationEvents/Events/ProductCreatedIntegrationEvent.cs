using EventBus.Events;

namespace Home.Api.IntegrationEvents;

public record class ProductCreatedIntegrationEvent : IntegrationEvent
{
    public ProductCreatedIntegrationEvent(int productId, string name, string? description, decimal price, int categoryId, string categoryName)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Price = price;
        CategoryId = categoryId;
        CategoryName = categoryName;
    }

    public int ProductId { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
}
using InventoryService.Messaging;

namespace InventoryService.Tests;

public class FakeProductPricePublisher : IProductPricePublisher
{
    public List<(string ProductName, decimal UnitPrice, string Currency)> Published { get; } = new();

    public Task PublishPriceChangedAsync(string productName, decimal unitPrice, string currency, CancellationToken cancellationToken = default)
    {
        Published.Add((productName, unitPrice, currency));
        return Task.CompletedTask;
    }
}

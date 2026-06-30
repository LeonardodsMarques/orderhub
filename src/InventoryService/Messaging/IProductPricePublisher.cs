namespace InventoryService.Messaging;

public interface IProductPricePublisher
{
    Task PublishPriceChangedAsync(string productName, decimal unitPrice, string currency, CancellationToken cancellationToken = default);
}

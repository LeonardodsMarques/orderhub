using MassTransit;
using OrderHub.Contracts;

namespace InventoryService.Messaging;

public class MassTransitProductPricePublisher : IProductPricePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitProductPricePublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishPriceChangedAsync(string productName, decimal unitPrice, string currency, CancellationToken cancellationToken = default)
    {
        return _publishEndpoint.Publish(new ProductPriceChanged(productName, new MoneyDto(unitPrice, currency)), cancellationToken);
    }
}

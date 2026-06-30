using InventoryService.Services;
using MassTransit;
using OrderHub.Contracts;

namespace InventoryService.Consumers;

public class InventoryConsumer : IConsumer<OrderCreated>
{
    private readonly IStockService _stockService;
    private readonly ILogger<InventoryConsumer> _logger;

    public InventoryConsumer(IStockService stockService, ILogger<InventoryConsumer> logger)
    {
        _stockService = stockService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var order = context.Message;
        var items = order.Items.Select(i => (i.ProductName, i.Quantity));

        var (reserved, missingProduct) = _stockService.TryReserveItems(items);

        if (reserved)
        {
            _logger.LogInformation("Stock reserved for order {OrderId}", order.OrderId);
            await context.Publish(new StockReserved(order.OrderId), context.CancellationToken);
        }
        else
        {
            _logger.LogWarning("Out of stock for product {ProductName} in order {OrderId}", missingProduct, order.OrderId);
            await context.Publish(new OutOfStock(order.OrderId, missingProduct ?? "unknown"), context.CancellationToken);
        }
    }
}

using InventoryService.Services;
using MassTransit;
using OrderHub.Contracts;

namespace InventoryService.Consumers;

public class OrderCancelledConsumer : IConsumer<OrderCancelled>
{
    private readonly IStockService _stockService;
    private readonly ILogger<OrderCancelledConsumer> _logger;

    public OrderCancelledConsumer(IStockService stockService, ILogger<OrderCancelledConsumer> logger)
    {
        _stockService = stockService;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderCancelled> context)
    {
        var order = context.Message;
        var items = order.Items.Select(i => (i.ProductName, i.Quantity));

        _stockService.ReleaseItems(items);
        _logger.LogInformation("Estoque liberado para o pedido {OrderId}", order.OrderId);

        return Task.CompletedTask;
    }
}

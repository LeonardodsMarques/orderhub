using MassTransit;
using OrderHub.Contracts;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Data;

namespace OrderService.Infrastructure.Messaging.Consumers;

public class ProductPriceChangedConsumer : IConsumer<ProductPriceChanged>
{
    private readonly OrderDbContext _dbContext;

    public ProductPriceChangedConsumer(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ProductPriceChanged> context)
    {
        var message = context.Message;
        var existing = await _dbContext.ProductPrices.FindAsync(new object[] { message.ProductName }, context.CancellationToken);

        if (existing is null)
        {
            _dbContext.ProductPrices.Add(new ProductPrice
            {
                ProductName = message.ProductName,
                UnitPrice = message.UnitPrice.Amount,
                Currency = message.UnitPrice.Currency
            });
        }
        else
        {
            existing.UnitPrice = message.UnitPrice.Amount;
            existing.Currency = message.UnitPrice.Currency;
        }

        await _dbContext.SaveChangesAsync(context.CancellationToken);
    }
}

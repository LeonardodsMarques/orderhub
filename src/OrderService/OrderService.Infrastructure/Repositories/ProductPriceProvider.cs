using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Data;

namespace OrderService.Infrastructure.Repositories;

public class ProductPriceProvider : IProductPriceProvider
{
    private readonly OrderDbContext _dbContext;

    public ProductPriceProvider(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ProductPrice?> GetByProductNameAsync(string productName, CancellationToken cancellationToken = default)
    {
        return _dbContext.ProductPrices
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductName == productName, cancellationToken);
    }
}

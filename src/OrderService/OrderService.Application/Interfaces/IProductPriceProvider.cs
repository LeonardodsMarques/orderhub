using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces;

public interface IProductPriceProvider
{
    Task<ProductPrice?> GetByProductNameAsync(string productName, CancellationToken cancellationToken = default);
}

using InventoryService.Data;
using InventoryService.Entities;
using InventoryService.Messaging;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Services;

public class EfStockService : IStockService
{
    private readonly InventoryDbContext _dbContext;
    private readonly IProductPricePublisher _pricePublisher;

    public EfStockService(InventoryDbContext dbContext, IProductPricePublisher pricePublisher)
    {
        _dbContext = dbContext;
        _pricePublisher = pricePublisher;
    }

    public (bool Reserved, string? MissingProduct) TryReserveItems(IEnumerable<(string ProductName, int Quantity)> items)
    {
        var requested = items.ToList();

        foreach (var (productName, quantity) in requested)
        {
            var item = _dbContext.StockItems.Find(productName);
            if (item is null || item.Quantity < quantity)
                return (false, productName);
        }

        foreach (var (productName, quantity) in requested)
        {
            var item = _dbContext.StockItems.Find(productName);
            item!.Quantity -= quantity;
        }

        _dbContext.SaveChanges();
        return (true, null);
    }

    public IReadOnlyDictionary<string, int> GetStock()
    {
        return _dbContext.StockItems
            .AsNoTracking()
            .ToDictionary(i => i.ProductName, i => i.Quantity);
    }

    public IReadOnlyList<StockItem> GetProducts()
    {
        return _dbContext.StockItems
            .AsNoTracking()
            .OrderBy(i => i.ProductName)
            .ToList();
    }

    public void SetQuantity(string productName, int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("A quantidade não pode ser negativa.", nameof(quantity));

        var item = _dbContext.StockItems.Find(productName);
        if (item is null)
        {
            item = new StockItem
            {
                ProductName = productName,
                UnitPrice = 0,
                Currency = "BRL"
            };
            _dbContext.StockItems.Add(item);
        }

        item.Quantity = quantity;
        _dbContext.SaveChanges();
    }

    public async Task SetPriceAsync(string productName, decimal unitPrice, string currency, CancellationToken cancellationToken = default)
    {
        if (unitPrice < 0)
            throw new ArgumentException("O preço não pode ser negativo.", nameof(unitPrice));

        var item = _dbContext.StockItems.Find(productName);
        if (item is null)
            throw new InvalidOperationException($"Produto '{productName}' não encontrado no estoque.");

        if (item.UnitPrice == unitPrice && item.Currency == currency)
            return;

        item.UnitPrice = unitPrice;
        item.Currency = currency;
        _dbContext.SaveChanges();

        await _pricePublisher.PublishPriceChangedAsync(productName, unitPrice, currency, cancellationToken);
    }

    public bool RemoveProduct(string productName)
    {
        var item = _dbContext.StockItems.Find(productName);
        if (item is null)
            return false;

        _dbContext.StockItems.Remove(item);
        _dbContext.SaveChanges();
        return true;
    }
}

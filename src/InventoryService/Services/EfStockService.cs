using InventoryService.Data;
using InventoryService.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Services;

public class EfStockService : IStockService
{
    private readonly InventoryDbContext _dbContext;

    public EfStockService(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
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

    public void SetQuantity(string productName, int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("A quantidade não pode ser negativa.", nameof(quantity));

        var item = _dbContext.StockItems.Find(productName);
        if (item is null)
        {
            item = new StockItem { ProductName = productName };
            _dbContext.StockItems.Add(item);
        }

        item.Quantity = quantity;
        _dbContext.SaveChanges();
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

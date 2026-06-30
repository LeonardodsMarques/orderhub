using InventoryService.Entities;
using System.Collections.ObjectModel;

namespace InventoryService.Services;

public interface IStockService
{
    (bool Reserved, string? MissingProduct) TryReserveItems(IEnumerable<(string ProductName, int Quantity)> items);
    void ReleaseItems(IEnumerable<(string ProductName, int Quantity)> items);
    IReadOnlyDictionary<string, int> GetStock();
    IReadOnlyList<StockItem> GetProducts();
    void SetQuantity(string productName, int quantity);
    Task SetPriceAsync(string productName, decimal unitPrice, string currency, CancellationToken cancellationToken = default);
    bool RemoveProduct(string productName);
}

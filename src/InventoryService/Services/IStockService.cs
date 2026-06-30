using System.Collections.ObjectModel;

namespace InventoryService.Services;

public interface IStockService
{
    (bool Reserved, string? MissingProduct) TryReserveItems(IEnumerable<(string ProductName, int Quantity)> items);
    IReadOnlyDictionary<string, int> GetStock();
    void SetQuantity(string productName, int quantity);
    bool RemoveProduct(string productName);
}

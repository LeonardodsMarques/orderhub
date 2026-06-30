namespace InventoryService.Services;

public class InMemoryStockService : IStockService
{
    private readonly Dictionary<string, int> _stock = new()
    {
        ["Widget"] = 10,
        ["Gadget"] = 5,
        ["Book"] = 20,
        ["Item"] = 100,
        ["Tool"] = 50,
        ["Thing"] = 30
    };

    private readonly object _lock = new();

    public (bool Reserved, string? MissingProduct) TryReserveItems(IEnumerable<(string ProductName, int Quantity)> items)
    {
        var requested = items.ToList();

        lock (_lock)
        {
            foreach (var (productName, quantity) in requested)
            {
                if (!_stock.TryGetValue(productName, out var available) || available < quantity)
                    return (false, productName);
            }

            foreach (var (productName, quantity) in requested)
            {
                _stock[productName] -= quantity;
            }

            return (true, null);
        }
    }
}

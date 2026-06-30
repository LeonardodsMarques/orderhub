using InventoryService.Entities;

namespace InventoryService.Data;

public static class StockSeed
{
    public static IReadOnlyList<StockItem> Items { get; } = new List<StockItem>
    {
        new() { ProductName = "Widget", Quantity = 10 },
        new() { ProductName = "Gadget", Quantity = 5 },
        new() { ProductName = "Book", Quantity = 20 },
        new() { ProductName = "Item", Quantity = 100 },
        new() { ProductName = "Tool", Quantity = 50 },
        new() { ProductName = "Thing", Quantity = 30 }
    };
}

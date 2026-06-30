using InventoryService.Entities;

namespace InventoryService.Data;

public static class StockSeed
{
    public static IReadOnlyList<StockItem> Items { get; } = new List<StockItem>
    {
        new() { ProductName = "Mouse", Quantity = 10 },
        new() { ProductName = "Teclado", Quantity = 5 },
        new() { ProductName = "Livro", Quantity = 20 },
        new() { ProductName = "Ferramenta", Quantity = 50 },
    };
}

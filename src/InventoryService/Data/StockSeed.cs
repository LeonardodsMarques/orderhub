using InventoryService.Entities;

namespace InventoryService.Data;

public static class StockSeed
{
    public static IReadOnlyList<StockItem> Items { get; } = new List<StockItem>
    {
        new() { ProductName = "Mouse", Quantity = 10, UnitPrice = 49.90m, Currency = "BRL" },
        new() { ProductName = "Teclado", Quantity = 5, UnitPrice = 129.90m, Currency = "BRL" },
        new() { ProductName = "Livro", Quantity = 20, UnitPrice = 39.90m, Currency = "BRL" },
        new() { ProductName = "Ferramenta", Quantity = 50, UnitPrice = 89.90m, Currency = "BRL" }
    };
}

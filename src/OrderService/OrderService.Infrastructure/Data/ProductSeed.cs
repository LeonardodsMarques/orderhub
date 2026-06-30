using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data;

public static class ProductSeed
{
    public static IReadOnlyList<ProductPrice> Items { get; } = new List<ProductPrice>
    {
        new() { ProductName = "Mouse", UnitPrice = 49.90m, Currency = "BRL" },
        new() { ProductName = "Teclado", UnitPrice = 129.90m, Currency = "BRL" },
        new() { ProductName = "Livro", UnitPrice = 39.90m, Currency = "BRL" },
        new() { ProductName = "Ferramenta", UnitPrice = 89.90m, Currency = "BRL" }
    };
}

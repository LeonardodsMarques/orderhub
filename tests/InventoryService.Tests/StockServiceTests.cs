using InventoryService.Data;
using InventoryService.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InventoryService.Tests;

public class StockServiceTests
{
    private static InventoryDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new InventoryDbContext(options);
        context.Database.EnsureCreated();
        context.StockItems.AddRange(StockSeed.Items);
        context.SaveChanges();
        return context;
    }

    [Fact]
    public void TryReserveItems_WhenStockAvailable_ReservesAndReturnsTrue()
    {
        using var context = CreateContext();
        var stock = new EfStockService(context, new FakeProductPricePublisher());

        var (reserved, missing) = stock.TryReserveItems(new[]
        {
            ("Livro", 2),
            ("Mouse", 3)
        });

        Assert.True(reserved);
        Assert.Null(missing);
    }

    [Fact]
    public void TryReserveItems_WhenStockInsufficient_ReturnsFalseWithProductName()
    {
        using var context = CreateContext();
        var stock = new EfStockService(context, new FakeProductPricePublisher());

        var (reserved, missing) = stock.TryReserveItems(new[]
        {
            ("Teclado", 10)
        });

        Assert.False(reserved);
        Assert.Equal("Teclado", missing);
    }

    [Fact]
    public void TryReserveItems_WhenProductUnknown_ReturnsFalseWithProductName()
    {
        using var context = CreateContext();
        var stock = new EfStockService(context, new FakeProductPricePublisher());

        var (reserved, missing) = stock.TryReserveItems(new[]
        {
            ("NonExistent", 1)
        });

        Assert.False(reserved);
        Assert.Equal("NonExistent", missing);
    }

    [Fact]
    public void TryReserveItems_AfterReservation_StockIsDecremented()
    {
        using var context = CreateContext();
        var stock = new EfStockService(context, new FakeProductPricePublisher());

        stock.TryReserveItems(new[] { ("Livro", 5) });
        var (reserved, missing) = stock.TryReserveItems(new[] { ("Livro", 16) });

        Assert.False(reserved);
        Assert.Equal("Livro", missing);
    }

    [Fact]
    public void GetProducts_ReturnsAllProductsWithPrices()
    {
        using var context = CreateContext();
        var stock = new EfStockService(context, new FakeProductPricePublisher());

        var products = stock.GetProducts();

        Assert.Equal(4, products.Count);
        Assert.Contains(products, p => p.ProductName == "Livro" && p.UnitPrice == 39.90m && p.Currency == "BRL");
    }

    [Fact]
    public async Task SetPriceAsync_WhenPriceChanges_UpdatesPriceAndPublishesEvent()
    {
        using var context = CreateContext();
        var publisher = new FakeProductPricePublisher();
        var stock = new EfStockService(context, publisher);

        await stock.SetPriceAsync("Livro", 49.90m, "USD");

        var product = context.StockItems.Single(p => p.ProductName == "Livro");
        Assert.Equal(49.90m, product.UnitPrice);
        Assert.Equal("USD", product.Currency);
        Assert.Single(publisher.Published);
        Assert.Equal(("Livro", 49.90m, "USD"), publisher.Published[0]);
    }

    [Fact]
    public async Task SetPriceAsync_WhenPriceUnchanged_DoesNotPublishEvent()
    {
        using var context = CreateContext();
        var publisher = new FakeProductPricePublisher();
        var stock = new EfStockService(context, publisher);

        await stock.SetPriceAsync("Mouse", 49.90m, "BRL");

        Assert.Empty(publisher.Published);
    }
}

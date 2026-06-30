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
        var stock = new EfStockService(context);

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
        var stock = new EfStockService(context);

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
        var stock = new EfStockService(context);

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
        var stock = new EfStockService(context);

        stock.TryReserveItems(new[] { ("Livro", 5) });
        var (reserved, missing) = stock.TryReserveItems(new[] { ("Livro", 16) });

        Assert.False(reserved);
        Assert.Equal("Livro", missing);
    }
}

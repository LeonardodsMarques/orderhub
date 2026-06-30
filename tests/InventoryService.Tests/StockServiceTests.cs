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
            ("Book", 2),
            ("Widget", 3)
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
            ("Gadget", 10)
        });

        Assert.False(reserved);
        Assert.Equal("Gadget", missing);
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

        stock.TryReserveItems(new[] { ("Book", 5) });
        var (reserved, missing) = stock.TryReserveItems(new[] { ("Book", 16) });

        Assert.False(reserved);
        Assert.Equal("Book", missing);
    }
}

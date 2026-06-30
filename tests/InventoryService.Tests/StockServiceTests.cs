using InventoryService.Services;
using Xunit;

namespace InventoryService.Tests;

public class StockServiceTests
{
    [Fact]
    public void TryReserveItems_WhenStockAvailable_ReservesAndReturnsTrue()
    {
        var stock = new InMemoryStockService();

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
        var stock = new InMemoryStockService();

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
        var stock = new InMemoryStockService();

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
        var stock = new InMemoryStockService();

        stock.TryReserveItems(new[] { ("Book", 5) });
        var (reserved, missing) = stock.TryReserveItems(new[] { ("Book", 16) });

        Assert.False(reserved);
        Assert.Equal("Book", missing);
    }
}

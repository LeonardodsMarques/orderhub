using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using Xunit;

namespace OrderService.Domain.Tests;

public class OrderTests
{
    private static OrderItem CreateItem(string product, int quantity, decimal price, string currency = "USD")
    {
        return new OrderItem(product, quantity, new Money(price, currency));
    }

    [Fact]
    public void Constructor_WithItems_CreatesPendingOrderAndCalculatesTotal()
    {
        var order = new Order("John", "john@example.com", new[]
        {
            CreateItem("Book", 2, 10.00m)
        });

        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(20.00m, order.TotalAmount.Amount);
        Assert.Equal("USD", order.TotalAmount.Currency);
    }

    [Fact]
    public void Constructor_WithoutItems_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Order("John", "john@example.com", Array.Empty<OrderItem>()));
    }

    [Fact]
    public void Confirm_WhenPending_SetsConfirmed()
    {
        var order = new Order("John", "john@example.com", new[] { CreateItem("Book", 1, 10.00m) });

        order.Confirm();

        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Theory]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Cancelled)]
    public void Confirm_WhenNotPending_ThrowsInvalidOperationException(OrderStatus status)
    {
        var order = new Order("John", "john@example.com", new[] { CreateItem("Book", 1, 10.00m) });
        if (status == OrderStatus.Confirmed) order.Confirm();
        else order.Cancel();

        Assert.Throws<InvalidOperationException>(() => order.Confirm());
    }

    [Fact]
    public void Cancel_WhenPending_SetsCancelled()
    {
        var order = new Order("John", "john@example.com", new[] { CreateItem("Book", 1, 10.00m) });

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_WhenConfirmed_SetsCancelled()
    {
        var order = new Order("John", "john@example.com", new[] { CreateItem("Book", 1, 10.00m) });
        order.Confirm();

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ThrowsInvalidOperationException()
    {
        var order = new Order("John", "john@example.com", new[] { CreateItem("Book", 1, 10.00m) });
        order.Cancel();

        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }
}

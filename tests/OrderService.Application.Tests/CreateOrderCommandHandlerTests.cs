using Microsoft.EntityFrameworkCore;
using OrderHub.Contracts;
using OrderService.Application.Commands;
using OrderService.Application.Dtos;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.UnitOfWork;
using Xunit;

namespace OrderService.Application.Tests;

public class CreateOrderCommandHandlerTests
{
    private static OrderDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new OrderDbContext(options);
        context.Database.EnsureCreated();
        context.ProductPrices.AddRange(
            new ProductPrice { ProductName = "Mouse", UnitPrice = 49.90m, Currency = "BRL" },
            new ProductPrice { ProductName = "Teclado", UnitPrice = 129.90m, Currency = "BRL" }
        );
        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task HandleAsync_UsesLocalPriceAndCalculatesTotal()
    {
        using var context = CreateContext();
        var publisher = new FakeEventPublisher();
        var handler = new CreateOrderCommandHandler(
            new OrderRepository(context),
            new UnitOfWork(context),
            publisher,
            new ProductPriceProvider(context));

        var command = new CreateOrderCommand(
            "Maria",
            "maria@example.com",
            new List<CreateOrderItemDto>
            {
                new("Mouse", 2),
                new("Teclado", 1)
            });

        var id = await handler.HandleAsync(command);

        var order = context.Orders.Single(o => o.Id == id);
        Assert.Equal(2, order.Items.Count);
        Assert.Equal(229.70m, order.TotalAmount.Amount);
        Assert.Equal("BRL", order.TotalAmount.Currency);
        Assert.Single(publisher.Published.OfType<OrderCreated>());
    }

    [Fact]
    public async Task HandleAsync_WhenProductPriceMissing_ThrowsArgumentException()
    {
        using var context = CreateContext();
        var publisher = new FakeEventPublisher();
        var handler = new CreateOrderCommandHandler(
            new OrderRepository(context),
            new UnitOfWork(context),
            publisher,
            new ProductPriceProvider(context));

        var command = new CreateOrderCommand(
            "Maria",
            "maria@example.com",
            new List<CreateOrderItemDto> { new("Desconhecido", 1) });

        await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_GroupsDuplicateProducts()
    {
        using var context = CreateContext();
        var publisher = new FakeEventPublisher();
        var handler = new CreateOrderCommandHandler(
            new OrderRepository(context),
            new UnitOfWork(context),
            publisher,
            new ProductPriceProvider(context));

        var command = new CreateOrderCommand(
            "Maria",
            "maria@example.com",
            new List<CreateOrderItemDto>
            {
                new("Mouse", 2),
                new("Mouse", 3)
            });

        var id = await handler.HandleAsync(command);

        var order = context.Orders.Single(o => o.Id == id);
        Assert.Single(order.Items);
        Assert.Equal(5, order.Items[0].Quantity);
        Assert.Equal(249.50m, order.TotalAmount.Amount);
    }
}

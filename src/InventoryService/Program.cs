using InventoryService.Consumers;
using InventoryService.Services;
using MassTransit;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IStockService, InMemoryStockService>();

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<InventoryConsumer>();

    busConfigurator.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RabbitMq:Host"] ?? "localhost";
        var username = builder.Configuration["RabbitMq:Username"] ?? "guest";
        var password = builder.Configuration["RabbitMq:Password"] ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.Message<OrderHub.Contracts.OrderCreated>(x => x.SetEntityName("OrderCreated"));

        cfg.ReceiveEndpoint("inventory-order-created", e =>
        {
            e.ConfigureConsumer<InventoryConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapHealthChecks("/health");

app.Run();

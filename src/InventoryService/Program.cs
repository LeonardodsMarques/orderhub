using InventoryService.Consumers;
using InventoryService.Data;
using InventoryService.Messaging;
using InventoryService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<IStockService, EfStockService>();
builder.Services.AddScoped<IProductPricePublisher, MassTransitProductPricePublisher>();

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

app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    dbContext.Database.EnsureCreated();

    if (!dbContext.StockItems.Any())
    {
        dbContext.StockItems.AddRange(StockSeed.Items);
        dbContext.SaveChanges();
    }
}

app.Run();

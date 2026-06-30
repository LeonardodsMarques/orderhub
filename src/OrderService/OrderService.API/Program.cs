using Microsoft.EntityFrameworkCore;
using OrderService.Application.Commands;
using OrderService.Application.Dtos;
using OrderService.Application.Interfaces;
using OrderService.Application.Queries;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Messaging.Outbox;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "OrderService API",
        Version = "v1"
    });
});

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductPriceProvider, ProductPriceProvider>();
builder.Services.AddScoped<IOutboxStore, OutboxStore>();
builder.Services.AddHostedService<OutboxProcessor>();

builder.Services.AddScoped<ICommandHandler<CreateOrderCommand, Guid>, CreateOrderCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateOrderStatusCommand, bool>, UpdateOrderStatusCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>, GetOrdersQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetOrderByIdQuery, OrderDto>, GetOrderByIdQueryHandler>();

builder.Services.AddMassTransitWithRabbitMq(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<OrderService.API.Middleware.ExceptionMiddleware>();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    dbContext.Database.Migrate();

    if (!dbContext.ProductPrices.Any())
    {
        dbContext.ProductPrices.AddRange(ProductSeed.Items);
        dbContext.SaveChanges();
    }
}

app.Run();

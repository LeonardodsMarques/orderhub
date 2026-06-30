using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderHub.Contracts;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Messaging.Consumers;
using RabbitMQ.Client;

namespace OrderService.Infrastructure.Messaging;

public static class MassTransitConfiguration
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<StockReservedConsumer>();
            busConfigurator.AddConsumer<OutOfStockConsumer>();

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["RabbitMq:Host"] ?? "localhost";
                var username = configuration["RabbitMq:Username"] ?? "guest";
                var password = configuration["RabbitMq:Password"] ?? "guest";

                cfg.Host(host, "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.Message<OrderCreated>(x => x.SetEntityName("OrderCreated"));
                cfg.Publish<OrderCreated>(p => p.ExchangeType = ExchangeType.Fanout);

                cfg.ReceiveEndpoint("order-stock-reserved", e =>
                {
                    e.ConfigureConsumer<StockReservedConsumer>(context);
                });

                cfg.ReceiveEndpoint("order-out-of-stock", e =>
                {
                    e.ConfigureConsumer<OutOfStockConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IEventPublisher, MassTransitEventBus>();

        return services;
    }
}

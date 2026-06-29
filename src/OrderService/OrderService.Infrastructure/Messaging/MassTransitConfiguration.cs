using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces;
using OrderService.Domain.Events;
using RabbitMQ.Client;

namespace OrderService.Infrastructure.Messaging;

public static class MassTransitConfiguration
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfigurator =>
        {
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
                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IOrderEventPublisher, MassTransitEventBus>();

        return services;
    }
}

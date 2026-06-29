using MassTransit;
using OrderService.Application.Interfaces;
using OrderService.Domain.Events;

namespace OrderService.Infrastructure.Messaging;

public class MassTransitEventBus : IOrderEventPublisher
{
    private readonly IBus _bus;

    public MassTransitEventBus(IBus bus)
    {
        _bus = bus;
    }

    public Task PublishAsync(OrderCreated evt, CancellationToken cancellationToken = default)
    {
        return _bus.Publish(evt, cancellationToken);
    }
}

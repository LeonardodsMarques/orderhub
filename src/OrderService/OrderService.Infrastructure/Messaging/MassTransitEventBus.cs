using MassTransit;
using OrderService.Application.Interfaces;

namespace OrderService.Infrastructure.Messaging;

public class MassTransitEventBus : IEventPublisher
{
    private readonly IBus _bus;

    public MassTransitEventBus(IBus bus)
    {
        _bus = bus;
    }

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        return _bus.Publish(message, cancellationToken);
    }
}

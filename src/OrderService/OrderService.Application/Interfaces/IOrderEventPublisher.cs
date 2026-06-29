using OrderService.Domain.Events;

namespace OrderService.Application.Interfaces;

public interface IOrderEventPublisher
{
    Task PublishAsync(OrderCreated evt, CancellationToken cancellationToken = default);
}

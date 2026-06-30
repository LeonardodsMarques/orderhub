using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces;

public interface IOutboxStore
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
}

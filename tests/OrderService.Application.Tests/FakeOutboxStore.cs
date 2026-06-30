using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.Application.Tests;

public class FakeOutboxStore : IOutboxStore
{
    public List<OutboxMessage> Messages { get; } = new();

    public Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        Messages.Add(message);
        return Task.CompletedTask;
    }
}

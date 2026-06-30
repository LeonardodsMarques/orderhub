using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Data;

namespace OrderService.Infrastructure.Messaging.Outbox;

public class OutboxStore : IOutboxStore
{
    private readonly OrderDbContext _context;

    public OutboxStore(OrderDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        _context.OutboxMessages.Add(message);
        return Task.CompletedTask;
    }
}

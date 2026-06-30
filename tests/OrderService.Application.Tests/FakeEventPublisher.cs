using OrderService.Application.Interfaces;

namespace OrderService.Application.Tests;

public class FakeEventPublisher : IEventPublisher
{
    public List<object> Published { get; } = new();

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        Published.Add(message);
        return Task.CompletedTask;
    }
}

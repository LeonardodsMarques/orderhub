using OrderService.Application.Dtos;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Events;

namespace OrderService.Application.Commands;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderEventPublisher _eventPublisher;

    public CreateOrderCommandHandler(
        IOrderRepository repository,
        IUnitOfWork unitOfWork,
        IOrderEventPublisher eventPublisher)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Items.Count == 0)
            throw new ArgumentException("Order must contain at least one item.", nameof(command));

        var items = command.Items
            .Select(i => new OrderItem(i.ProductName, i.Quantity, new Money(i.UnitPrice, i.Currency)))
            .ToList();

        var order = new Order(command.CustomerName, command.CustomerEmail, items);

        await _repository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var evt = new OrderCreated(order.Id, order.CustomerEmail, order.TotalAmount, order.CreatedAt);
        await _eventPublisher.PublishAsync(evt, cancellationToken);

        return order.Id;
    }
}

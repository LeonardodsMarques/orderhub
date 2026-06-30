using OrderHub.Contracts;
using OrderService.Application.Interfaces;
using OrderService.Domain.Enums;

namespace OrderService.Application.Commands;

public class UpdateOrderStatusCommandHandler : ICommandHandler<UpdateOrderStatusCommand, bool>
{
    private readonly IOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventPublisher _eventPublisher;

    public UpdateOrderStatusCommandHandler(IOrderRepository repository, IUnitOfWork unitOfWork, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
    }

    public async Task<bool> HandleAsync(UpdateOrderStatusCommand command, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<OrderStatus>(command.Status, ignoreCase: true, out var status))
            throw new ArgumentException($"Status inválido: {command.Status}", nameof(command));

        var order = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (order is null)
            return false;

        switch (status)
        {
            case OrderStatus.Confirmed:
                order.Confirm();
                break;
            case OrderStatus.Cancelled:
                order.Cancel();
                break;
            default:
                throw new ArgumentException($"Status {status} não é suportado para atualizações manuais.");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (status == OrderStatus.Cancelled)
        {
            var items = order.Items
                .Select(i => new OrderCancelledItem(i.ProductName, i.Quantity))
                .ToList();

            await _eventPublisher.PublishAsync(new OrderCancelled(order.Id, items), cancellationToken);
        }

        return true;
    }
}

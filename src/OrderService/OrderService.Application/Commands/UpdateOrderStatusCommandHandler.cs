using OrderService.Application.Interfaces;
using OrderService.Domain.Enums;

namespace OrderService.Application.Commands;

public class UpdateOrderStatusCommandHandler : ICommandHandler<UpdateOrderStatusCommand, bool>
{
    private readonly IOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderStatusCommandHandler(IOrderRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(UpdateOrderStatusCommand command, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<OrderStatus>(command.Status, ignoreCase: true, out var status))
            throw new ArgumentException($"Invalid status: {command.Status}", nameof(command));

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
                throw new ArgumentException($"Status {status} is not supported for manual updates.");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

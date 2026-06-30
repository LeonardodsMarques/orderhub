using MassTransit;
using OrderHub.Contracts;
using OrderService.Application.Interfaces;

namespace OrderService.Infrastructure.Messaging.Consumers;

public class StockReservedConsumer : IConsumer<StockReserved>
{
    private readonly IOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public StockReservedConsumer(IOrderRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<StockReserved> context)
    {
        var order = await _repository.GetByIdAsync(context.Message.OrderId, context.CancellationToken);
        if (order is null)
            return;

        order.Confirm();
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}

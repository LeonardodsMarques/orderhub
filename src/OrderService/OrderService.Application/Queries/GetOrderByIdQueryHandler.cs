using OrderService.Application.Dtos;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.Application.Queries;

public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IOrderRepository _repository;

    public GetOrderByIdQueryHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderDto?> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(query.Id, cancellationToken);
        return order is null ? null : Map(order);
    }

    private static OrderDto Map(Order order)
    {
        return new OrderDto(
            order.Id,
            order.CustomerName,
            order.CustomerEmail,
            order.Items.Select(i => new OrderItemDto(
                i.ProductName,
                i.Quantity,
                new MoneyDto(i.UnitPrice.Amount, i.UnitPrice.Currency))).ToList(),
            new MoneyDto(order.TotalAmount.Amount, order.TotalAmount.Currency),
            order.Status.ToString(),
            order.CreatedAt);
    }
}

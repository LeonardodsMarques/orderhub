using OrderService.Application.Dtos;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.Application.Queries;

public class GetOrdersQueryHandler : IQueryHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>
{
    private readonly IOrderRepository _repository;

    public GetOrdersQueryHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<OrderDto>?> HandleAsync(GetOrdersQuery query, CancellationToken cancellationToken = default)
    {
        var orders = await _repository.GetAllAsync(query.Skip, query.Take, cancellationToken);
        return orders.Select(Map).ToList();
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

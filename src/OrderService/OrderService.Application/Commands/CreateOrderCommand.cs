using OrderService.Application.Dtos;

namespace OrderService.Application.Commands;

public record CreateOrderCommand(string CustomerName, string CustomerEmail, IReadOnlyList<CreateOrderItemDto> Items);

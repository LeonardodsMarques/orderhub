namespace OrderService.Application.Dtos;

public record OrderDto(
    Guid Id,
    string CustomerName,
    string CustomerEmail,
    IReadOnlyList<OrderItemDto> Items,
    MoneyDto TotalAmount,
    string Status,
    DateTime CreatedAt);

namespace OrderService.Application.Dtos;

public record OrderItemDto(string ProductName, int Quantity, MoneyDto UnitPrice);

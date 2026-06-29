using OrderService.Application.Dtos;

namespace OrderService.API.Dtos;

public record CreateOrderRequest(string CustomerName, string CustomerEmail, List<CreateOrderItemDto> Items);

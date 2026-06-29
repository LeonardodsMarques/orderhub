namespace OrderService.Application.Commands;

public record UpdateOrderStatusCommand(Guid Id, string Status);

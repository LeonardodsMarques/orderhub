using OrderService.Domain.Entities;

namespace OrderService.Domain.Events;

public record OrderCreated(Guid OrderId, string CustomerEmail, Money TotalAmount, DateTime CreatedAt);

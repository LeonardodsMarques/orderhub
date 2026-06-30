namespace OrderHub.Contracts;

public record OrderCreated(
    Guid OrderId,
    string CustomerEmail,
    MoneyDto TotalAmount,
    DateTime CreatedAt,
    IReadOnlyList<OrderCreatedItem> Items);

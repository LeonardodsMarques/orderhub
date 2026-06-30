namespace OrderHub.Contracts;

public record OrderCancelled(
    Guid OrderId,
    IReadOnlyList<OrderCancelledItem> Items);

public record OrderCancelledItem(string ProductName, int Quantity);

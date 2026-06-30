namespace OrderHub.Contracts;

public record OutOfStock(Guid OrderId, string ProductName);

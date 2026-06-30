namespace OrderService.Domain.Entities;

public class ProductPrice
{
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "BRL";
}

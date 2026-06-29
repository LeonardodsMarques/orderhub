namespace OrderService.Domain.Entities;

public class Money
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;

    private Money() { }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
}

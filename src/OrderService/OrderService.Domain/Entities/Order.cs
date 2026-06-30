using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public List<OrderItem> Items { get; private set; } = new();
    public Money TotalAmount { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order() { }

    public Order(string customerName, string customerEmail, IEnumerable<OrderItem> items)
    {
        var itemList = items.ToList();
        if (itemList.Count == 0)
            throw new ArgumentException("O pedido deve conter pelo menos um item.", nameof(items));

        Id = Guid.NewGuid();
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        Items = itemList;
        TotalAmount = new Money(
            Items.Sum(i => i.Quantity * i.UnitPrice.Amount),
            Items.First().UnitPrice.Currency);
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Apenas pedidos pendentes podem ser confirmados.");

        Status = OrderStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("O pedido já está cancelado.");

        if (Status != OrderStatus.Pending && Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("O pedido não pode ser cancelado.");

        Status = OrderStatus.Cancelled;
    }
}

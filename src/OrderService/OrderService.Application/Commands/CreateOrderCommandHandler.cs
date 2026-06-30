using OrderHub.Contracts;
using OrderService.Application.Dtos;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.Application.Commands;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventPublisher _eventPublisher;
    private readonly IProductPriceProvider _priceProvider;

    public CreateOrderCommandHandler(
        IOrderRepository repository,
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher,
        IProductPriceProvider priceProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
        _priceProvider = priceProvider;
    }

    public async Task<Guid> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.CustomerName))
            throw new ArgumentException("O nome do cliente é obrigatório.", nameof(command));

        if (string.IsNullOrWhiteSpace(command.CustomerEmail) || !command.CustomerEmail.Contains('@'))
            throw new ArgumentException("E-mail do cliente inválido.", nameof(command));

        if (command.Items.Count == 0)
            throw new ArgumentException("O pedido deve conter pelo menos um item.", nameof(command));

        if (command.Items.Any(i => i.Quantity <= 0))
            throw new ArgumentException("A quantidade de cada item deve ser maior que zero.", nameof(command));

        var groupedItems = command.Items
            .GroupBy(i => i.ProductName)
            .Select(g => new { ProductName = g.Key, Quantity = g.Sum(i => i.Quantity) })
            .ToList();

        var orderItems = new List<OrderItem>();
        string? currency = null;

        foreach (var item in groupedItems)
        {
            var price = await _priceProvider.GetByProductNameAsync(item.ProductName, cancellationToken);
            if (price is null)
                throw new ArgumentException($"Preço não encontrado para o produto '{item.ProductName}'.", nameof(command));

            currency ??= price.Currency;
            if (price.Currency != currency)
                throw new ArgumentException("Todos os itens do pedido devem usar a mesma moeda.", nameof(command));

            orderItems.Add(new OrderItem(item.ProductName, item.Quantity, new Money(price.UnitPrice, price.Currency)));
        }

        var order = new Order(command.CustomerName, command.CustomerEmail, orderItems);

        await _repository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var evt = new OrderCreated(
            order.Id,
            order.CustomerEmail,
            new OrderHub.Contracts.MoneyDto(order.TotalAmount.Amount, order.TotalAmount.Currency),
            order.CreatedAt,
            order.Items.Select(i => new OrderCreatedItem(i.ProductName, i.Quantity)).ToList());

        await _eventPublisher.PublishAsync(evt, cancellationToken);

        return order.Id;
    }
}

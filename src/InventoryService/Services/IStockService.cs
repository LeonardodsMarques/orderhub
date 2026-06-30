namespace InventoryService.Services;

public interface IStockService
{
    (bool Reserved, string? MissingProduct) TryReserveItems(IEnumerable<(string ProductName, int Quantity)> items);
}

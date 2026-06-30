using InventoryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

[ApiController]
[Route("stock")]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<StockProductDto>> GetProducts()
    {
        var products = _stockService.GetProducts()
            .Select(p => new StockProductDto(p.ProductName, p.Quantity, p.UnitPrice, p.Currency))
            .ToList();

        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> UpsertProduct([FromBody] UpsertStockRequest request)
    {
        _stockService.SetQuantity(request.ProductName, request.Quantity);
        await _stockService.SetPriceAsync(request.ProductName, request.UnitPrice, request.Currency);
        return NoContent();
    }

    [HttpDelete("{productName}")]
    public IActionResult RemoveProduct(string productName)
    {
        var removed = _stockService.RemoveProduct(productName);
        return removed ? NoContent() : NotFound();
    }
}

public record StockProductDto(string ProductName, int Quantity, decimal UnitPrice, string Currency);

public record UpsertStockRequest(string ProductName, int Quantity, decimal UnitPrice, string Currency);

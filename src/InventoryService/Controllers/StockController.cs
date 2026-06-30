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
    public ActionResult<IReadOnlyDictionary<string, int>> GetStock()
    {
        return Ok(_stockService.GetStock());
    }

    [HttpPost]
    public IActionResult SetQuantity([FromBody] SetStockRequest request)
    {
        _stockService.SetQuantity(request.ProductName, request.Quantity);
        return NoContent();
    }

    [HttpDelete("{productName}")]
    public IActionResult RemoveProduct(string productName)
    {
        var removed = _stockService.RemoveProduct(productName);
        return removed ? NoContent() : NotFound();
    }
}

public record SetStockRequest(string ProductName, int Quantity);

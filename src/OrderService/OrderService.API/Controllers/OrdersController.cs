using Microsoft.AspNetCore.Mvc;
using OrderService.API.Dtos;
using OrderService.Application.Commands;
using OrderService.Application.Dtos;
using OrderService.Application.Interfaces;
using OrderService.Application.Queries;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly ICommandHandler<CreateOrderCommand, Guid> _createHandler;
    private readonly ICommandHandler<UpdateOrderStatusCommand, bool> _updateHandler;
    private readonly IQueryHandler<GetOrdersQuery, IReadOnlyList<OrderDto>> _getOrdersHandler;
    private readonly IQueryHandler<GetOrderByIdQuery, OrderDto> _getByIdHandler;

    public OrdersController(
        ICommandHandler<CreateOrderCommand, Guid> createHandler,
        ICommandHandler<UpdateOrderStatusCommand, bool> updateHandler,
        IQueryHandler<GetOrdersQuery, IReadOnlyList<OrderDto>> getOrdersHandler,
        IQueryHandler<GetOrderByIdQuery, OrderDto> getByIdHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _getOrdersHandler = getOrdersHandler;
        _getByIdHandler = getByIdHandler;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(
            request.CustomerName,
            request.CustomerEmail,
            request.Items);

        var id = await _createHandler.HandleAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAll(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10,
        CancellationToken cancellationToken = default)
    {
        var orders = await _getOrdersHandler.HandleAsync(
            new GetOrdersQuery(skip, take),
            cancellationToken);

        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var order = await _getByIdHandler.HandleAsync(
            new GetOrderByIdQuery(id),
            cancellationToken);

        return order is null ? NotFound() : Ok(order);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _updateHandler.HandleAsync(
            new UpdateOrderStatusCommand(id, request.Status),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }
}

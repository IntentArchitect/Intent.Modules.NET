using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Wolverine.AspNetCore.Controllers.Application;
using Wolverine.AspNetCore.Controllers.Application.Common.Pagination;
using Wolverine.AspNetCore.Controllers.Application.CreateOrder;
using Wolverine.AspNetCore.Controllers.Application.DeleteOrder;
using Wolverine.AspNetCore.Controllers.Application.GetOrderById;
using Wolverine.AspNetCore.Controllers.Application.GetOrders;
using Wolverine.AspNetCore.Controllers.Application.GetOrderStatistics;
using Wolverine.AspNetCore.Controllers.Application.PlaceOrder;
using Wolverine.AspNetCore.Controllers.Application.UpdateOrder;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Api.Controllers
{
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMessageBus _sender;

        public OrdersController(IMessageBus sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/orders")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDto>> CreateOrder(
            [FromBody] CreateOrderCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.InvokeAsync<OrderDto>(command, cancellationToken);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="401">Unauthorized request.</response>
        /// <response code="403">Forbidden request.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/orders/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteOrder([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            await _sender.InvokeAsync(new DeleteOrderCommand(id: id), cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Confirms/places an order by invoking the Order.PlaceOrder domain operation, which raises OrderPlacedDomainEvent (handled by the default domain-event handler).
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("api/orders/{id}/place")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PlaceOrder([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            await _sender.InvokeAsync(new PlaceOrderCommand(id: id), cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/orders/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateOrder(
            [FromRoute] Guid id,
            [FromBody] UpdateOrderCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == Guid.Empty)
            {
                command.Id = id;
            }

            if (id != command.Id)
            {
                return BadRequest();
            }

            await _sender.InvokeAsync(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified OrderDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No OrderDto could be found with the provided parameters.</response>
        [HttpGet("api/orders/{id}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDto>> GetOrderById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.InvokeAsync<OrderDto>(new GetOrderByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Paged get-all. Convention-based (no domain interaction) so CQRS.CRUD's paged strategy implements it.
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;OrderDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/orders")]
        [ProducesResponseType(typeof(PagedResult<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<OrderDto>>> GetOrders(
            [FromQuery][Required] int pageNo = 1,
            [FromQuery][Required] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.InvokeAsync<PagedResult<OrderDto>>(new GetOrdersQuery(pageNo: pageNo, pageSize: pageSize), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Custom query with no domain interaction. Handler body computes aggregate statistics (implemented by hand).
        /// </summary>
        /// <response code="200">Returns the specified OrderStatisticsDto.</response>
        [HttpGet("api/orders/statistics")]
        [ProducesResponseType(typeof(OrderStatisticsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderStatisticsDto>> GetOrderStatistics(CancellationToken cancellationToken = default)
        {
            var result = await _sender.InvokeAsync<OrderStatisticsDto>(new GetOrderStatisticsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
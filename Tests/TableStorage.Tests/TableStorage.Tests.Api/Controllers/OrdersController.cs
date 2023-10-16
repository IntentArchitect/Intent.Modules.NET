using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TableStorage.Tests.Application.Orders;
using TableStorage.Tests.Application.Orders.CreateOrder;
using TableStorage.Tests.Application.Orders.DeleteOrder;
using TableStorage.Tests.Application.Orders.GetOrderById;
using TableStorage.Tests.Application.Orders.GetOrders;
using TableStorage.Tests.Application.Orders.UpdateOrder;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace TableStorage.Tests.Api.Controllers
{
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ISender _mediator;

        public OrdersController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/order/{partitionKey}/{rowKey}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateOrder(
            [FromRoute] string partitionKey,
            [FromRoute] string rowKey,
            [FromBody] CreateOrderCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.PartitionKey == default)
            {
                command.PartitionKey = partitionKey;
            }

            if (command.RowKey == default)
            {
                command.RowKey = rowKey;
            }

            if (partitionKey != command.PartitionKey)
            {
                return BadRequest();
            }

            if (rowKey != command.RowKey)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpDelete("api/order/{partitionKey}/{rowKey}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteOrder(
            [FromRoute] string partitionKey,
            [FromRoute] string rowKey,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteOrderCommand(partitionKey: partitionKey, rowKey: rowKey), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("api/order/{partitionKey}/{rowKey}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateOrder(
            [FromRoute] string partitionKey,
            [FromRoute] string rowKey,
            [FromBody] UpdateOrderCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.PartitionKey == default)
            {
                command.PartitionKey = partitionKey;
            }

            if (command.RowKey == default)
            {
                command.RowKey = rowKey;
            }

            if (partitionKey != command.PartitionKey)
            {
                return BadRequest();
            }

            if (rowKey != command.RowKey)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified OrderDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No OrderDto could be found with the provided parameters.</response>
        [HttpGet("api/order/{partitionKey}/{rowKey}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDto>> GetOrderById(
            [FromRoute] string partitionKey,
            [FromRoute] string rowKey,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(partitionKey: partitionKey, rowKey: rowKey), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;OrderDto&gt;.</response>
        [HttpGet("api/order")]
        [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<OrderDto>>> GetOrders(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetOrdersQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ObjectMappingTest.Application.Orders;
using ObjectMappingTest.Application.Orders.GetOrderById;
using ObjectMappingTest.Application.Orders.GetOrderDetail;
using ObjectMappingTest.Application.Orders.GetOrderSummaryById;
using ObjectMappingTest.Application.Orders.GetOrderWithCustomer;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace ObjectMappingTest.Api.Controllers
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
            var result = await _mediator.Send(new GetOrderById(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified OrderDetailDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No OrderDetailDto could be found with the provided parameters.</response>
        [HttpGet("api/orders/{id}/detail")]
        [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDetailDto>> GetOrderDetail(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetOrderDetail(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified OrderSummaryDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No OrderSummaryDto could be found with the provided parameters.</response>
        [HttpGet("api/orders/{id}/summary")]
        [ProducesResponseType(typeof(OrderSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderSummaryDto>> GetOrderSummaryById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetOrderSummaryById(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified OrderWithCustomerDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No OrderWithCustomerDto could be found with the provided parameters.</response>
        [HttpGet("api/orders/{id}/with-customer")]
        [ProducesResponseType(typeof(OrderWithCustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderWithCustomerDto>> GetOrderWithCustomer(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetOrderWithCustomer(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using WolverineEventing.Publish.RabbitMQ.Application.Orders.FailOrder;
using WolverineEventing.Publish.RabbitMQ.Application.Orders.RequestOrderProcessing;
using WolverineEventing.Publish.RabbitMQ.Application.Orders.ShipOrder;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace WolverineEventing.Publish.RabbitMQ.Api.Controllers
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
        /// Publishes FailingOrderEvent. Exists only to drive the retry/dead-letter path.
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/orders/fail")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> FailOrder(
            [FromBody] FailOrderCommand command,
            CancellationToken cancellationToken = default)
        {
            await _sender.InvokeAsync(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// Requests order processing by sending the ProcessOrderCommand integration command.
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/orders/request-processing")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RequestOrderProcessing(
            [FromBody] RequestOrderProcessingCommand command,
            CancellationToken cancellationToken = default)
        {
            await _sender.InvokeAsync(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// Ships an order and publishes OrderShippedEvent. Exercises the publish path end to end.
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/orders/ship")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ShipOrder(
            [FromBody] ShipOrderCommand command,
            CancellationToken cancellationToken = default)
        {
            await _sender.InvokeAsync(command, cancellationToken);
            return Created(string.Empty, null);
        }
    }
}
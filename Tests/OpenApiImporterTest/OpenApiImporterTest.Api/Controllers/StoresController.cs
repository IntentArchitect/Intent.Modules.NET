using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OpenApiImporterTest.Application.Stores;
using OpenApiImporterTest.Application.Stores.CreateOrder;
using OpenApiImporterTest.Application.Stores.DeleteOrder;
using OpenApiImporterTest.Application.Stores.GetInventory;
using OpenApiImporterTest.Application.Stores.GetOrder;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace OpenApiImporterTest.Api.Controllers
{
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly ISender _mediator;

        public StoresController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("/store/order")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Order>> CreateOrder(
            [FromBody] CreateOrderCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("/store/order/{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteOrder([FromRoute] int orderId, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteOrderCommand(orderId: orderId), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified Dictionary&lt;string, int&gt;.</response>
        /// <response code="401">Unauthorized request.</response>
        /// <response code="403">Forbidden request.</response>
        [HttpGet("/store/inventory")]
        [Authorize]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Dictionary<string, int>>> GetInventory(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetInventoryQuery(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified Order.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No Order could be found with the provided parameters.</response>
        [HttpGet("/store/order/{orderId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Order>> GetOrder(
            [FromRoute] int orderId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetOrderQuery(orderId: orderId), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
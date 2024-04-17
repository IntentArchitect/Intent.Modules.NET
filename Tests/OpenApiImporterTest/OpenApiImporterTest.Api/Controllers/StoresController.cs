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
using OpenApiImporterTest.Application.Stores.CreateStoreOrder;
using OpenApiImporterTest.Application.Stores.DeleteStoreOrder;
using OpenApiImporterTest.Application.Stores.GetStoreInventory;
using OpenApiImporterTest.Application.Stores.GetStoreOrder;

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
        public async Task<ActionResult<Order>> CreateStoreOrder(
            [FromBody] CreateStoreOrderCommand command,
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
        public async Task<ActionResult> DeleteStoreOrder(
            [FromRoute] int orderId,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteStoreOrderCommand(orderId: orderId), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified Dictionary&lt;string, int&gt;.</response>
        [HttpGet("/store/inventory")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Dictionary<string, int>>> GetStoreInventory(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetStoreInventoryQuery(), cancellationToken);
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
        public async Task<ActionResult<Order>> GetStoreOrder(
            [FromRoute] int orderId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetStoreOrderQuery(orderId: orderId), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
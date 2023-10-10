using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Api.Controllers.ResponseTypes;
using CleanArchitecture.SingleFiles.Application.CosmosInvoices;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Api.Controllers
{
    [ApiController]
    public class CosmosInvoicesController : ControllerBase
    {
        private readonly ISender _mediator;

        public CosmosInvoicesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/cosmos-invoice")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateCosmosInvoice(
            [FromBody] CreateCosmosInvoiceCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetCosmosInvoiceById), new { id = result }, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("api/cosmos-invoice/{cosmosInvoiceId}/cosmos-line")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateCosmosInvoiceCosmosLine(
            [FromRoute] string cosmosInvoiceId,
            [FromBody] CreateCosmosInvoiceCosmosLineCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.CosmosInvoiceId == default)
            {
                command.CosmosInvoiceId = cosmosInvoiceId;
            }

            if (cosmosInvoiceId != command.CosmosInvoiceId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return result == null ? NotFound() : CreatedAtAction(nameof(GetCosmosInvoiceCosmosLineById), new { cosmosInvoiceId = cosmosInvoiceId, id = result }, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/cosmos-invoice/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCosmosInvoice(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteCosmosInvoiceCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/cosmos-invoice/{cosmosInvoiceId}/cosmos-line/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCosmosInvoiceCosmosLine(
            [FromRoute] string cosmosInvoiceId,
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteCosmosInvoiceCosmosLineCommand(cosmosInvoiceId: cosmosInvoiceId, id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/cosmos-invoice/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCosmosInvoice(
            [FromRoute] string id,
            [FromBody] UpdateCosmosInvoiceCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == default)
            {
                command.Id = id;
            }
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/cosmos-invoice/{cosmosInvoiceId}/cosmos-line/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCosmosInvoiceCosmosLine(
            [FromRoute] string cosmosInvoiceId,
            [FromRoute] string id,
            [FromBody] UpdateCosmosInvoiceCosmosLineCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.CosmosInvoiceId == default)
            {
                command.CosmosInvoiceId = cosmosInvoiceId;
            }

            if (command.Id == default)
            {
                command.Id = id;
            }
            if (cosmosInvoiceId != command.CosmosInvoiceId)
            {
                return BadRequest();
            }
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified CosmosInvoiceDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No CosmosInvoiceDto could be found with the provided parameters.</response>
        [HttpGet("api/cosmos-invoice/{id}")]
        [ProducesResponseType(typeof(CosmosInvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CosmosInvoiceDto>> GetCosmosInvoiceById(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCosmosInvoiceByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified CosmosInvoiceCosmosLineDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No CosmosInvoiceCosmosLineDto could be found with the provided parameters.</response>
        [HttpGet("api/cosmos-invoice/{cosmosInvoiceId}/cosmos-line/{id}")]
        [ProducesResponseType(typeof(CosmosInvoiceCosmosLineDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CosmosInvoiceCosmosLineDto>> GetCosmosInvoiceCosmosLineById(
            [FromRoute] string cosmosInvoiceId,
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCosmosInvoiceCosmosLineByIdQuery(cosmosInvoiceId: cosmosInvoiceId, id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;CosmosInvoiceCosmosLineDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No List&lt;CosmosInvoiceCosmosLineDto&gt; could be found with the provided parameters.</response>
        [HttpGet("api/cosmos-invoice/{cosmosInvoiceId}/cosmos-line")]
        [ProducesResponseType(typeof(List<CosmosInvoiceCosmosLineDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CosmosInvoiceCosmosLineDto>>> GetCosmosInvoiceCosmosLines(
            [FromRoute] string cosmosInvoiceId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCosmosInvoiceCosmosLinesQuery(cosmosInvoiceId: cosmosInvoiceId), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;CosmosInvoiceDto&gt;.</response>
        [HttpGet("api/cosmos-invoice")]
        [ProducesResponseType(typeof(List<CosmosInvoiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CosmosInvoiceDto>>> GetCosmosInvoices(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCosmosInvoicesQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
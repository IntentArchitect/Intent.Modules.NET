using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Api.Controllers.ResponseTypes;
using CleanArchitecture.SingleFiles.Application.DaprInvoices;
using CleanArchitecture.SingleFiles.Application.DaprInvoices.CreateDaprInvoice;
using CleanArchitecture.SingleFiles.Application.DaprInvoices.CreateDaprInvoiceDaprLine;
using CleanArchitecture.SingleFiles.Application.DaprInvoices.DeleteDaprInvoice;
using CleanArchitecture.SingleFiles.Application.DaprInvoices.DeleteDaprInvoiceDaprLine;
using CleanArchitecture.SingleFiles.Application.DaprInvoices.GetDaprInvoiceById;
using CleanArchitecture.SingleFiles.Application.DaprInvoices.GetDaprInvoiceDaprLineById;
using CleanArchitecture.SingleFiles.Application.DaprInvoices.GetDaprInvoiceDaprLines;
using CleanArchitecture.SingleFiles.Application.DaprInvoices.GetDaprInvoices;
using CleanArchitecture.SingleFiles.Application.DaprInvoices.UpdateDaprInvoice;
using CleanArchitecture.SingleFiles.Application.DaprInvoices.UpdateDaprInvoiceDaprLine;
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
    public class DaprInvoicesController : ControllerBase
    {
        private readonly ISender _mediator;

        public DaprInvoicesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/dapr-invoice")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateDaprInvoice(
            [FromBody] CreateDaprInvoiceCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetDaprInvoiceById), new { id = result }, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("api/dapr-invoice/{daprInvoiceId}/dapr-line")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateDaprInvoiceDaprLine(
            [FromRoute] string daprInvoiceId,
            [FromBody] CreateDaprInvoiceDaprLineCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.DaprInvoiceId == default)
            {
                command.DaprInvoiceId = daprInvoiceId;
            }

            if (daprInvoiceId != command.DaprInvoiceId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return result == null ? NotFound() : CreatedAtAction(nameof(GetDaprInvoiceDaprLineById), new { daprInvoiceId = daprInvoiceId, id = result }, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/dapr-invoice/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteDaprInvoice(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteDaprInvoiceCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/dapr-invoice/{daprInvoiceId}/dapr-line/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteDaprInvoiceDaprLine(
            [FromRoute] string daprInvoiceId,
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteDaprInvoiceDaprLineCommand(daprInvoiceId: daprInvoiceId, id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/dapr-invoice/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateDaprInvoice(
            [FromRoute] string id,
            [FromBody] UpdateDaprInvoiceCommand command,
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
        [HttpPut("api/dapr-invoice/{daprInvoiceId}/dapr-line/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateDaprInvoiceDaprLine(
            [FromRoute] string daprInvoiceId,
            [FromRoute] string id,
            [FromBody] UpdateDaprInvoiceDaprLineCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.DaprInvoiceId == default)
            {
                command.DaprInvoiceId = daprInvoiceId;
            }

            if (command.Id == default)
            {
                command.Id = id;
            }
            if (daprInvoiceId != command.DaprInvoiceId)
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
        /// <response code="200">Returns the specified DaprInvoiceDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No DaprInvoiceDto could be found with the provided parameters.</response>
        [HttpGet("api/dapr-invoice/{id}")]
        [ProducesResponseType(typeof(DaprInvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DaprInvoiceDto>> GetDaprInvoiceById(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDaprInvoiceByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified DaprInvoiceDaprLineDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No DaprInvoiceDaprLineDto could be found with the provided parameters.</response>
        [HttpGet("api/dapr-invoice/{daprInvoiceId}/dapr-line/{id}")]
        [ProducesResponseType(typeof(DaprInvoiceDaprLineDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DaprInvoiceDaprLineDto>> GetDaprInvoiceDaprLineById(
            [FromRoute] string daprInvoiceId,
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDaprInvoiceDaprLineByIdQuery(daprInvoiceId: daprInvoiceId, id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;DaprInvoiceDaprLineDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No List&lt;DaprInvoiceDaprLineDto&gt; could be found with the provided parameters.</response>
        [HttpGet("api/dapr-invoice/{daprInvoiceId}/dapr-line")]
        [ProducesResponseType(typeof(List<DaprInvoiceDaprLineDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<DaprInvoiceDaprLineDto>>> GetDaprInvoiceDaprLines(
            [FromRoute] string daprInvoiceId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDaprInvoiceDaprLinesQuery(daprInvoiceId: daprInvoiceId), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;DaprInvoiceDto&gt;.</response>
        [HttpGet("api/dapr-invoice")]
        [ProducesResponseType(typeof(List<DaprInvoiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<DaprInvoiceDto>>> GetDaprInvoices(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDaprInvoicesQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
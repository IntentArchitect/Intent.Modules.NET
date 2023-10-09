using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Api.Controllers.ResponseTypes;
using CleanArchitecture.SingleFiles.Application.EfInvoices;
using CleanArchitecture.SingleFiles.Application.EfInvoices.CreateEfInvoice;
using CleanArchitecture.SingleFiles.Application.EfInvoices.CreateEfInvoiceEfLine;
using CleanArchitecture.SingleFiles.Application.EfInvoices.DeleteEfInvoice;
using CleanArchitecture.SingleFiles.Application.EfInvoices.DeleteEfInvoiceEfLine;
using CleanArchitecture.SingleFiles.Application.EfInvoices.GetEfInvoiceById;
using CleanArchitecture.SingleFiles.Application.EfInvoices.GetEfInvoiceEfLineById;
using CleanArchitecture.SingleFiles.Application.EfInvoices.GetEfInvoiceEfLines;
using CleanArchitecture.SingleFiles.Application.EfInvoices.GetEfInvoices;
using CleanArchitecture.SingleFiles.Application.EfInvoices.UpdateEfInvoice;
using CleanArchitecture.SingleFiles.Application.EfInvoices.UpdateEfInvoiceEfLine;
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
    public class EfInvoicesController : ControllerBase
    {
        private readonly ISender _mediator;

        public EfInvoicesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/ef-invoice")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateEfInvoice(
            [FromBody] CreateEfInvoiceCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetEfInvoiceById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("api/ef-invoice/{efInvoicesId}/ef-line")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateEfInvoiceEfLine(
            [FromRoute] Guid efInvoicesId,
            [FromBody] CreateEfInvoiceEfLineCommand command,
            CancellationToken cancellationToken = default)
        {
            if (efInvoicesId != command.EfInvoicesId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetEfInvoiceEfLineById), new { efInvoicesId = efInvoicesId, id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/ef-invoice/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteEfInvoice([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteEfInvoiceCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/ef-invoice/{efInvoicesId}/ef-line/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteEfInvoiceEfLine(
            [FromRoute] Guid efInvoicesId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteEfInvoiceEfLineCommand(efInvoicesId: efInvoicesId, id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/ef-invoice/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEfInvoice(
            [FromRoute] Guid id,
            [FromBody] UpdateEfInvoiceCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == default)
            {
                command.SetId(id);
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
        [HttpPut("api/ef-invoice/{efInvoicesId}/ef-line/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEfInvoiceEfLine(
            [FromRoute] Guid efInvoicesId,
            [FromRoute] Guid id,
            [FromBody] UpdateEfInvoiceEfLineCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.EfInvoicesId == default)
            {
                command.SetEfInvoicesId(efInvoicesId);
            }

            if (command.Id == default)
            {
                command.SetId(id);
            }
            if (efInvoicesId != command.EfInvoicesId)
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
        /// <response code="200">Returns the specified EfInvoiceDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No EfInvoiceDto could be found with the provided parameters.</response>
        [HttpGet("api/ef-invoice/{id}")]
        [ProducesResponseType(typeof(EfInvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EfInvoiceDto>> GetEfInvoiceById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetEfInvoiceByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified EfInvoiceEfLineDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No EfInvoiceEfLineDto could be found with the provided parameters.</response>
        [HttpGet("api/ef-invoice/{efInvoicesId}/ef-line/{id}")]
        [ProducesResponseType(typeof(EfInvoiceEfLineDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EfInvoiceEfLineDto>> GetEfInvoiceEfLineById(
            [FromRoute] Guid efInvoicesId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetEfInvoiceEfLineByIdQuery(efInvoicesId: efInvoicesId, id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;EfInvoiceEfLineDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No List&lt;EfInvoiceEfLineDto&gt; could be found with the provided parameters.</response>
        [HttpGet("api/ef-invoice/{efInvoicesId}/ef-line")]
        [ProducesResponseType(typeof(List<EfInvoiceEfLineDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EfInvoiceEfLineDto>>> GetEfInvoiceEfLines(
            [FromRoute] Guid efInvoicesId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetEfInvoiceEfLinesQuery(efInvoicesId: efInvoicesId), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;EfInvoiceDto&gt;.</response>
        [HttpGet("api/ef-invoice")]
        [ProducesResponseType(typeof(List<EfInvoiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EfInvoiceDto>>> GetEfInvoices(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetEfInvoicesQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
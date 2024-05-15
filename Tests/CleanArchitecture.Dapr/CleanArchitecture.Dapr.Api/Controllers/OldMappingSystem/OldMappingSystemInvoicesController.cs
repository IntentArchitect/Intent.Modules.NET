using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Api.Controllers.ResponseTypes;
using CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices;
using CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.CreateInvoice;
using CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.CreateInvoiceInvoiceLine;
using CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.DeleteInvoice;
using CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.DeleteInvoiceInvoiceLine;
using CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.GetInvoiceById;
using CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.GetInvoiceInvoiceLineById;
using CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.GetInvoiceInvoiceLines;
using CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.GetInvoices;
using CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.UpdateInvoice;
using CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.UpdateInvoiceInvoiceLine;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CleanArchitecture.Dapr.Api.Controllers.OldMappingSystem
{
    [ApiController]
    public class OldMappingSystemInvoicesController : ControllerBase
    {
        private readonly ISender _mediator;

        public OldMappingSystemInvoicesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/invoices")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateInvoice(
            [FromBody] CreateInvoiceCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetInvoiceById), new { id = result }, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("api/invoices/{invoiceId}/invoice-lines")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateInvoiceInvoiceLine(
            [FromRoute] string invoiceId,
            [FromBody] CreateInvoiceInvoiceLineCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.InvoiceId == default)
            {
                command.InvoiceId = invoiceId;
            }

            if (invoiceId != command.InvoiceId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return result == null ? NotFound() : CreatedAtAction(nameof(GetInvoiceInvoiceLineById), new { invoiceId = invoiceId, id = result }, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/invoices/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteInvoice([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteInvoiceCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/invoices/{invoiceId}/invoice-lines/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteInvoiceInvoiceLine(
            [FromRoute] string invoiceId,
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteInvoiceInvoiceLineCommand(invoiceId: invoiceId, id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/invoices/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateInvoice(
            [FromRoute] string id,
            [FromBody] UpdateInvoiceCommand command,
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
        [HttpPut("api/invoices/{invoiceId}/invoice-lines/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateInvoiceInvoiceLine(
            [FromRoute] string invoiceId,
            [FromRoute] string id,
            [FromBody] UpdateInvoiceInvoiceLineCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.InvoiceId == default)
            {
                command.InvoiceId = invoiceId;
            }

            if (command.Id == default)
            {
                command.Id = id;
            }
            if (invoiceId != command.InvoiceId)
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
        /// <response code="200">Returns the specified InvoiceDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No InvoiceDto could be found with the provided parameters.</response>
        [HttpGet("api/invoices/{id}")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InvoiceDto>> GetInvoiceById(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetInvoiceByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified InvoiceInvoiceLineDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No InvoiceInvoiceLineDto could be found with the provided parameters.</response>
        [HttpGet("api/invoices/{invoiceId}/invoice-lines/{id}")]
        [ProducesResponseType(typeof(InvoiceInvoiceLineDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InvoiceInvoiceLineDto>> GetInvoiceInvoiceLineById(
            [FromRoute] string invoiceId,
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetInvoiceInvoiceLineByIdQuery(invoiceId: invoiceId, id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;InvoiceInvoiceLineDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No List&lt;InvoiceInvoiceLineDto&gt; could be found with the provided parameters.</response>
        [HttpGet("api/invoices/{invoiceId}/invoice-lines")]
        [ProducesResponseType(typeof(List<InvoiceInvoiceLineDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<InvoiceInvoiceLineDto>>> GetInvoiceInvoiceLines(
            [FromRoute] string invoiceId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetInvoiceInvoiceLinesQuery(invoiceId: invoiceId), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;InvoiceDto&gt;.</response>
        [HttpGet("api/invoices")]
        [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<InvoiceDto>>> GetInvoices(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetInvoicesQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
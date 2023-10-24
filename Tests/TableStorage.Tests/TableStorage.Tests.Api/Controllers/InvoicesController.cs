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
using TableStorage.Tests.Application.Invoices;
using TableStorage.Tests.Application.Invoices.CreateInvoice;
using TableStorage.Tests.Application.Invoices.DeleteInvoice;
using TableStorage.Tests.Application.Invoices.GetInvoiceById;
using TableStorage.Tests.Application.Invoices.GetInvoices;
using TableStorage.Tests.Application.Invoices.UpdateInvoice;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace TableStorage.Tests.Api.Controllers
{
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly ISender _mediator;

        public InvoicesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/invoice/{partitionKey}/{rowKey}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateInvoice(
            [FromRoute] string partitionKey,
            [FromRoute] string rowKey,
            [FromBody] CreateInvoiceCommand command,
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
        [HttpDelete("api/invoice/{partitionKey}/{rowKey}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteInvoice(
            [FromRoute] string partitionKey,
            [FromRoute] string rowKey,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteInvoiceCommand(partitionKey: partitionKey, rowKey: rowKey), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("api/invoice/{partitionKey}/{rowKey}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateInvoice(
            [FromRoute] string partitionKey,
            [FromRoute] string rowKey,
            [FromBody] UpdateInvoiceCommand command,
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
        /// <response code="200">Returns the specified InvoiceDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No InvoiceDto could be found with the provided parameters.</response>
        [HttpGet("api/invoice/{partitionKey}/{rowKey}")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InvoiceDto>> GetInvoiceById(
            [FromRoute] string partitionKey,
            [FromRoute] string rowKey,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetInvoiceByIdQuery(partitionKey: partitionKey, rowKey: rowKey), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;InvoiceDto&gt;.</response>
        [HttpGet("api/invoice")]
        [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<InvoiceDto>>> GetInvoices(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetInvoicesQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
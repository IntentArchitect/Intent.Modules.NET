using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Api.Controllers.ResponseTypes;
using CleanArchitecture.SingleFiles.Application.MongoInvoices;
using CleanArchitecture.SingleFiles.Application.MongoInvoices.CreateMongoInvoice;
using CleanArchitecture.SingleFiles.Application.MongoInvoices.CreateMongoInvoiceMongoLine;
using CleanArchitecture.SingleFiles.Application.MongoInvoices.DeleteMongoInvoice;
using CleanArchitecture.SingleFiles.Application.MongoInvoices.DeleteMongoInvoiceMongoLine;
using CleanArchitecture.SingleFiles.Application.MongoInvoices.GetMongoInvoiceById;
using CleanArchitecture.SingleFiles.Application.MongoInvoices.GetMongoInvoiceMongoLineById;
using CleanArchitecture.SingleFiles.Application.MongoInvoices.GetMongoInvoiceMongoLines;
using CleanArchitecture.SingleFiles.Application.MongoInvoices.GetMongoInvoices;
using CleanArchitecture.SingleFiles.Application.MongoInvoices.UpdateMongoInvoice;
using CleanArchitecture.SingleFiles.Application.MongoInvoices.UpdateMongoInvoiceMongoLine;
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
    public class MongoInvoicesController : ControllerBase
    {
        private readonly ISender _mediator;

        public MongoInvoicesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/mongo-invoice")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateMongoInvoice(
            [FromBody] CreateMongoInvoiceCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetMongoInvoiceById), new { id = result }, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("api/mongo-invoice/{mongoInvoiceId}/mongo-line")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateMongoInvoiceMongoLine(
            [FromRoute] string mongoInvoiceId,
            [FromBody] CreateMongoInvoiceMongoLineCommand command,
            CancellationToken cancellationToken = default)
        {
            if (mongoInvoiceId != command.MongoInvoiceId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return result == null ? NotFound() : CreatedAtAction(nameof(GetMongoInvoiceMongoLineById), new { mongoInvoiceId = mongoInvoiceId, id = result }, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/mongo-invoice/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteMongoInvoice(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteMongoInvoiceCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/mongo-invoice/{mongoInvoiceId}/mongo-line/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteMongoInvoiceMongoLine(
            [FromRoute] string mongoInvoiceId,
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteMongoInvoiceMongoLineCommand(mongoInvoiceId: mongoInvoiceId, id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/mongo-invoice/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateMongoInvoice(
            [FromRoute] string id,
            [FromBody] UpdateMongoInvoiceCommand command,
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
        [HttpPut("api/mongo-invoice/{mongoInvoiceId}/mongo-line/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateMongoInvoiceMongoLine(
            [FromRoute] string mongoInvoiceId,
            [FromRoute] string id,
            [FromBody] UpdateMongoInvoiceMongoLineCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.MongoInvoiceId == default)
            {
                command.SetMongoInvoiceId(mongoInvoiceId);
            }

            if (command.Id == default)
            {
                command.SetId(id);
            }
            if (mongoInvoiceId != command.MongoInvoiceId)
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
        /// <response code="200">Returns the specified MongoInvoiceDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No MongoInvoiceDto could be found with the provided parameters.</response>
        [HttpGet("api/mongo-invoice/{id}")]
        [ProducesResponseType(typeof(MongoInvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MongoInvoiceDto>> GetMongoInvoiceById(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetMongoInvoiceByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified MongoInvoiceMongoLineDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No MongoInvoiceMongoLineDto could be found with the provided parameters.</response>
        [HttpGet("api/mongo-invoice/{mongoInvoiceId}/mongo-line/{id}")]
        [ProducesResponseType(typeof(MongoInvoiceMongoLineDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MongoInvoiceMongoLineDto>> GetMongoInvoiceMongoLineById(
            [FromRoute] string mongoInvoiceId,
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetMongoInvoiceMongoLineByIdQuery(mongoInvoiceId: mongoInvoiceId, id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;MongoInvoiceMongoLineDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No List&lt;MongoInvoiceMongoLineDto&gt; could be found with the provided parameters.</response>
        [HttpGet("api/mongo-invoice/{mongoInvoiceId}/mongo-line")]
        [ProducesResponseType(typeof(List<MongoInvoiceMongoLineDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<MongoInvoiceMongoLineDto>>> GetMongoInvoiceMongoLines(
            [FromRoute] string mongoInvoiceId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetMongoInvoiceMongoLinesQuery(mongoInvoiceId: mongoInvoiceId), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;MongoInvoiceDto&gt;.</response>
        [HttpGet("api/mongo-invoice")]
        [ProducesResponseType(typeof(List<MongoInvoiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<MongoInvoiceDto>>> GetMongoInvoices(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetMongoInvoicesQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
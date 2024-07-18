using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Api.Controllers.ResponseTypes;
using AdvancedMappingCrudMongo.Tests.Application.ExternalDocs;
using AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.CreateExternalDoc;
using AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.DeleteExternalDoc;
using AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.GetExternalDocById;
using AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.GetExternalDocs;
using AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.UpdateExternalDoc;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Api.Controllers
{
    [ApiController]
    public class ExternalDocsController : ControllerBase
    {
        private readonly ISender _mediator;

        public ExternalDocsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/external-docs")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<long>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<long>>> CreateExternalDoc(
            [FromBody] CreateExternalDocCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetExternalDocById), new { id = result }, new JsonResponse<long>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/external-docs/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteExternalDoc(
            [FromRoute] long id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteExternalDocCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/external-docs/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateExternalDoc(
            [FromRoute] long id,
            [FromBody] UpdateExternalDocCommand command,
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
        /// <response code="200">Returns the specified ExternalDocDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No ExternalDocDto could be found with the provided parameters.</response>
        [HttpGet("api/external-docs/{id}")]
        [ProducesResponseType(typeof(ExternalDocDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExternalDocDto>> GetExternalDocById(
            [FromRoute] long id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetExternalDocByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ExternalDocDto&gt;.</response>
        [HttpGet("api/external-docs")]
        [ProducesResponseType(typeof(List<ExternalDocDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ExternalDocDto>>> GetExternalDocs(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetExternalDocsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
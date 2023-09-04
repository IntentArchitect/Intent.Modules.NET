using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Api.Controllers.ResponseTypes;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRoot;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRootImplicitKeyNestedComposition;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.DeleteImplicitKeyAggrRoot;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.DeleteImplicitKeyAggrRootImplicitKeyNestedComposition;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootById;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootImplicitKeyNestedCompositionById;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootImplicitKeyNestedCompositions;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRoot;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRootImplicitKeyNestedComposition;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Api.Controllers
{
    [ApiController]
    public class ImplicitKeyAggrRootsController : ControllerBase
    {
        private readonly ISender _mediator;

        public ImplicitKeyAggrRootsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/implicit-key-aggr-roots")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateImplicitKeyAggrRoot(
            [FromBody] CreateImplicitKeyAggrRootCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetImplicitKeyAggrRootById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("api/implicit-key-aggr-roots/{implicitKeyAggrRootId}/ImplicitKeyNestedCompositions")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateImplicitKeyAggrRootImplicitKeyNestedComposition(
            [FromRoute] Guid implicitKeyAggrRootId,
            [FromBody] CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand command,
            CancellationToken cancellationToken = default)
        {
            if (implicitKeyAggrRootId != command.ImplicitKeyAggrRootId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetImplicitKeyAggrRootImplicitKeyNestedCompositionById), new { implicitKeyAggrRootId = implicitKeyAggrRootId, id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/implicit-key-aggr-roots/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteImplicitKeyAggrRoot(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteImplicitKeyAggrRootCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/implicit-key-aggr-roots/{implicitKeyAggrRootId}/ImplicitKeyNestedCompositions/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteImplicitKeyAggrRootImplicitKeyNestedComposition(
            [FromRoute] Guid implicitKeyAggrRootId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand(implicitKeyAggrRootId: implicitKeyAggrRootId, id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/implicit-key-aggr-roots/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateImplicitKeyAggrRoot(
            [FromRoute] Guid id,
            [FromBody] UpdateImplicitKeyAggrRootCommand command,
            CancellationToken cancellationToken = default)
        {
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
        [HttpPut("api/implicit-key-aggr-roots/{implicitKeyAggrRootId}/ImplicitKeyNestedCompositions/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateImplicitKeyAggrRootImplicitKeyNestedComposition(
            [FromRoute] Guid implicitKeyAggrRootId,
            [FromRoute] Guid id,
            [FromBody] UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand command,
            CancellationToken cancellationToken = default)
        {
            if (implicitKeyAggrRootId != command.ImplicitKeyAggrRootId)
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
        /// <response code="200">Returns the specified ImplicitKeyAggrRootDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No ImplicitKeyAggrRootDto could be found with the provided parameters.</response>
        [HttpGet("api/implicit-key-aggr-roots/{id}")]
        [ProducesResponseType(typeof(ImplicitKeyAggrRootDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ImplicitKeyAggrRootDto>> GetImplicitKeyAggrRootById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetImplicitKeyAggrRootByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified ImplicitKeyAggrRootImplicitKeyNestedCompositionDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No ImplicitKeyAggrRootImplicitKeyNestedCompositionDto could be found with the provided parameters.</response>
        [HttpGet("api/implicit-key-aggr-roots/{implicitKeyAggrRootId}/ImplicitKeyNestedCompositions/{id}")]
        [ProducesResponseType(typeof(ImplicitKeyAggrRootImplicitKeyNestedCompositionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto>> GetImplicitKeyAggrRootImplicitKeyNestedCompositionById(
            [FromRoute] Guid implicitKeyAggrRootId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQuery(implicitKeyAggrRootId: implicitKeyAggrRootId, id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ImplicitKeyAggrRootImplicitKeyNestedCompositionDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No List&lt;ImplicitKeyAggrRootImplicitKeyNestedCompositionDto&gt; could be found with the provided parameters.</response>
        [HttpGet("api/implicit-key-aggr-roots/{implicitKeyAggrRootId}/ImplicitKeyNestedCompositions")]
        [ProducesResponseType(typeof(List<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto>>> GetImplicitKeyAggrRootImplicitKeyNestedCompositions(
            [FromRoute] Guid implicitKeyAggrRootId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQuery(implicitKeyAggrRootId: implicitKeyAggrRootId), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ImplicitKeyAggrRootDto&gt;.</response>
        [HttpGet("api/implicit-key-aggr-roots")]
        [ProducesResponseType(typeof(List<ImplicitKeyAggrRootDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ImplicitKeyAggrRootDto>>> GetImplicitKeyAggrRoots(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetImplicitKeyAggrRootsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
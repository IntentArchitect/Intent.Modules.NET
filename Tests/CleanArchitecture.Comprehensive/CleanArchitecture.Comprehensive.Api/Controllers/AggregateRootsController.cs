using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Api.Controllers.ResponseTypes;
using CleanArchitecture.Comprehensive.Application.AggregateRoots;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.CreateAggregateRoot;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.DeleteAggregateRoot;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.DeleteAggregateRootCompositeManyB;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.GetAggregateRootById;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.GetAggregateRootCompositeManyBById;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.GetAggregateRootCompositeManyBS;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.GetAggregateRoots;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.UpdateAggregateRoot;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.UpdateAggregateRootCompositeManyB;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.Controllers
{
    [ApiController]
    public class AggregateRootsController : ControllerBase
    {
        private readonly ISender _mediator;

        public AggregateRootsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/aggregate-roots", Name = "CreateAggregateRoot")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateAggregateRoot(
            [FromBody] CreateAggregateRootCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetAggregateRootById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("api/aggregate-roots/{aggregateRootId}/CompositeManyBS")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateAggregateRootCompositeManyB(
            [FromRoute] Guid aggregateRootId,
            [FromBody] CreateAggregateRootCompositeManyBCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.AggregateRootId == default)
            {
                command.AggregateRootId = aggregateRootId;
            }

            if (aggregateRootId != command.AggregateRootId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetAggregateRootCompositeManyBById), new { aggregateRootId = aggregateRootId, id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/aggregate-roots/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAggregateRoot(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteAggregateRootCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/aggregate-roots/{aggregateRootId}/CompositeManyBS/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAggregateRootCompositeManyB(
            [FromRoute] Guid aggregateRootId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteAggregateRootCompositeManyBCommand(aggregateRootId: aggregateRootId, id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/aggregate-roots/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateAggregateRoot(
            [FromRoute] Guid id,
            [FromBody] UpdateAggregateRootCommand command,
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
        [HttpPut("api/aggregate-roots/{aggregateRootId}/CompositeManyBS/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateAggregateRootCompositeManyB(
            [FromRoute] Guid aggregateRootId,
            [FromRoute] Guid id,
            [FromBody] UpdateAggregateRootCompositeManyBCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.AggregateRootId == default)
            {
                command.AggregateRootId = aggregateRootId;
            }

            if (command.Id == default)
            {
                command.Id = id;
            }
            if (aggregateRootId != command.AggregateRootId)
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
        /// <response code="200">Returns the specified AggregateRootDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No AggregateRootDto could be found with the provided parameters.</response>
        [HttpGet("api/aggregate-roots/{id}")]
        [ProducesResponseType(typeof(AggregateRootDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AggregateRootDto>> GetAggregateRootById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAggregateRootByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified AggregateRootCompositeManyBDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No AggregateRootCompositeManyBDto could be found with the provided parameters.</response>
        [HttpGet("api/aggregate-roots/{aggregateRootId}/CompositeManyBS/{id}")]
        [ProducesResponseType(typeof(AggregateRootCompositeManyBDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AggregateRootCompositeManyBDto>> GetAggregateRootCompositeManyBById(
            [FromRoute] Guid aggregateRootId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAggregateRootCompositeManyBByIdQuery(aggregateRootId: aggregateRootId, id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;AggregateRootCompositeManyBDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No List&lt;AggregateRootCompositeManyBDto&gt; could be found with the provided parameters.</response>
        [HttpGet("api/aggregate-roots/{aggregateRootId}/CompositeManyBS")]
        [ProducesResponseType(typeof(List<AggregateRootCompositeManyBDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AggregateRootCompositeManyBDto>>> GetAggregateRootCompositeManyBS(
            [FromRoute] Guid aggregateRootId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAggregateRootCompositeManyBSQuery(aggregateRootId: aggregateRootId), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;AggregateRootDto&gt;.</response>
        [HttpGet("api/aggregate-roots")]
        [ProducesResponseType(typeof(List<AggregateRootDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AggregateRootDto>>> GetAggregateRoots(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAggregateRootsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
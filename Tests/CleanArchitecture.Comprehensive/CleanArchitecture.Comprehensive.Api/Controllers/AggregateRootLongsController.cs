using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Api.Controllers.ResponseTypes;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs.CreateAggregateRootLong;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs.DeleteAggregateRootLong;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs.GetAggregateRootLongById;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs.GetAggregateRootLongs;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs.UpdateAggregateRootLong;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
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
    public class AggregateRootLongsController : ControllerBase
    {
        private readonly ISender _mediator;

        public AggregateRootLongsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/aggregate-root-longs")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<long>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<long>>> CreateAggregateRootLong(
            [FromBody] CreateAggregateRootLongCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetAggregateRootLongById), new { id = result }, new JsonResponse<long>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/aggregate-root-longs/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAggregateRootLong(
            [FromRoute] long id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteAggregateRootLongCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/aggregate-root-longs/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateAggregateRootLong(
            [FromRoute] long id,
            [FromBody] UpdateAggregateRootLongCommand command,
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
        /// <response code="200">Returns the specified AggregateRootLongDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No AggregateRootLongDto could be found with the provided parameters.</response>
        [HttpGet("api/aggregate-root-longs/{id}")]
        [ProducesResponseType(typeof(AggregateRootLongDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AggregateRootLongDto>> GetAggregateRootLongById(
            [FromRoute] long id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAggregateRootLongByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;AggregateRootLongDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/aggregate-root-longs")]
        [ProducesResponseType(typeof(PagedResult<AggregateRootLongDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<AggregateRootLongDto>>> GetAggregateRootLongs(
            [FromQuery] int pageNo,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAggregateRootLongsQuery(pageNo: pageNo, pageSize: pageSize), cancellationToken);
            return Ok(result);
        }
    }
}
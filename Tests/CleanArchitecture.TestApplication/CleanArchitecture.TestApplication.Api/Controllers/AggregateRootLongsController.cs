using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Api.Controllers.ResponseTypes;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.CreateAggregateRootLong;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.DeleteAggregateRootLong;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.GetAggregateRootLongById;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.GetAggregateRootLongs;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.UpdateAggregateRootLong;
using CleanArchitecture.TestApplication.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Api.Controllers
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<long>> CreateAggregateRootLong(
            [FromBody] CreateAggregateRootLongCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetAggregateRootLongById), new { id = result }, new JsonResponse<long>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpDelete("api/aggregate-root-longs/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAggregateRootLong([FromRoute] long id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteAggregateRootLongCommand { Id = id }, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("api/aggregate-root-longs/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateAggregateRootLong(
            [FromRoute] long id,
            [FromBody] UpdateAggregateRootLongCommand command,
            CancellationToken cancellationToken)
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
        /// <response code="200">Returns the specified AggregateRootLongDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an AggregateRootLongDto with the parameters provided.</response>
        [HttpGet("api/aggregate-root-longs/{id}")]
        [ProducesResponseType(typeof(AggregateRootLongDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AggregateRootLongDto>> GetAggregateRootLongById(
            [FromRoute] long id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAggregateRootLongByIdQuery { Id = id }, cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;AggregateRootLongDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an PagedResult&lt;AggregateRootLongDto&gt; with the parameters provided.</response>
        [HttpGet("api/aggregate-root-longs")]
        [ProducesResponseType(typeof(PagedResult<AggregateRootLongDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<AggregateRootLongDto>>> GetAggregateRootLongs(
            [FromBody] GetAggregateRootLongsQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }
    }
}
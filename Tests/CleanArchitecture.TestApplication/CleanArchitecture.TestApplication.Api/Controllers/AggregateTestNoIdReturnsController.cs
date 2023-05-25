using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns;
using CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns.CreateAggregateTestNoIdReturn;
using CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns.DeleteAggregateTestNoIdReturn;
using CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns.GetAggregateTestNoIdReturnById;
using CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns.GetAggregateTestNoIdReturns;
using CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns.UpdateAggregateTestNoIdReturn;
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
    public class AggregateTestNoIdReturnsController : ControllerBase
    {
        private readonly ISender _mediator;

        public AggregateTestNoIdReturnsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/aggregate-test-no-id-returns")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateAggregateTestNoIdReturn(
            [FromBody] CreateAggregateTestNoIdReturnCommand command,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpDelete("api/aggregate-test-no-id-returns/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAggregateTestNoIdReturn(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteAggregateTestNoIdReturnCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("api/aggregate-test-no-id-returns/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateAggregateTestNoIdReturn(
            [FromRoute] Guid id,
            [FromBody] UpdateAggregateTestNoIdReturnCommand command,
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
        /// <response code="200">Returns the specified AggregateTestNoIdReturnDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an AggregateTestNoIdReturnDto with the parameters provided.</response>
        [HttpGet("api/aggregate-test-no-id-returns/{id}")]
        [ProducesResponseType(typeof(AggregateTestNoIdReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AggregateTestNoIdReturnDto>> GetAggregateTestNoIdReturnById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAggregateTestNoIdReturnByIdQuery(id: id), cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;AggregateTestNoIdReturnDto&gt;.</response>
        [HttpGet("api/aggregate-test-no-id-returns")]
        [ProducesResponseType(typeof(List<AggregateTestNoIdReturnDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AggregateTestNoIdReturnDto>>> GetAggregateTestNoIdReturns(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAggregateTestNoIdReturnsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
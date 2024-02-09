using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Api.Controllers.ResponseTypes;
using IntegrationTesting.Tests.Application.DiffIds;
using IntegrationTesting.Tests.Application.DiffIds.CreateDiffId;
using IntegrationTesting.Tests.Application.DiffIds.DeleteDiffId;
using IntegrationTesting.Tests.Application.DiffIds.GetDiffIdById;
using IntegrationTesting.Tests.Application.DiffIds.GetDiffIds;
using IntegrationTesting.Tests.Application.DiffIds.UpdateDiffId;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace IntegrationTesting.Tests.Api.Controllers
{
    [ApiController]
    public class DiffIdsController : ControllerBase
    {
        private readonly ISender _mediator;

        public DiffIdsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/diff-id")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateDiffId(
            [FromBody] CreateDiffIdCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetDiffIdById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/diff-id/{myId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteDiffId([FromRoute] Guid myId, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteDiffIdCommand(myId: myId), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/diff-id/{myId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateDiffId(
            [FromRoute] Guid myId,
            [FromBody] UpdateDiffIdCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.MyId == default)
            {
                command.MyId = myId;
            }

            if (myId != command.MyId)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified DiffIdDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No DiffIdDto could be found with the provided parameters.</response>
        [HttpGet("api/diff-id/{id}")]
        [ProducesResponseType(typeof(DiffIdDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DiffIdDto>> GetDiffIdById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDiffIdByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;DiffIdDto&gt;.</response>
        [HttpGet("api/diff-id")]
        [ProducesResponseType(typeof(List<DiffIdDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<DiffIdDto>>> GetDiffIds(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDiffIdsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Api.Controllers.ResponseTypes;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.CreateWithCompositeKey;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.DeleteWithCompositeKey;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.GetWithCompositeKeyById;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.GetWithCompositeKeys;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.UpdateWithCompositeKey;
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
    public class WithCompositeKeysController : ControllerBase
    {
        private readonly ISender _mediator;

        public WithCompositeKeysController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/with-composite-key")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateWithCompositeKey(
            [FromBody] CreateWithCompositeKeyCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/with-composite-key/{key1Id}/{key2Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteWithCompositeKey(
            [FromRoute] Guid key1Id,
            [FromRoute] Guid key2Id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteWithCompositeKeyCommand(key1Id: key1Id, key2Id: key2Id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/with-composite-key/{key1Id}/{key2Id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateWithCompositeKey(
            [FromRoute] Guid key1Id,
            [FromRoute] Guid key2Id,
            [FromBody] UpdateWithCompositeKeyCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Key1Id == default)
            {
                command.Key1Id = key1Id;
            }

            if (command.Key2Id == default)
            {
                command.Key2Id = key2Id;
            }
            if (key1Id != command.Key1Id)
            {
                return BadRequest();
            }
            if (key2Id != command.Key2Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified WithCompositeKeyDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No WithCompositeKeyDto could be found with the provided parameters.</response>
        [HttpGet("api/with-composite-key/{key1Id}/{key2Id}")]
        [ProducesResponseType(typeof(WithCompositeKeyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<WithCompositeKeyDto>> GetWithCompositeKeyById(
            [FromRoute] Guid key1Id,
            [FromRoute] Guid key2Id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetWithCompositeKeyByIdQuery(key1Id: key1Id, key2Id: key2Id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;WithCompositeKeyDto&gt;.</response>
        [HttpGet("api/with-composite-key")]
        [ProducesResponseType(typeof(List<WithCompositeKeyDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<WithCompositeKeyDto>>> GetWithCompositeKeys(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetWithCompositeKeysQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
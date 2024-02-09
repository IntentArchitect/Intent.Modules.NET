using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Application.DtoReturns;
using IntegrationTesting.Tests.Application.DtoReturns.CreateDtoReturn;
using IntegrationTesting.Tests.Application.DtoReturns.DeleteDtoReturn;
using IntegrationTesting.Tests.Application.DtoReturns.GetDtoReturnById;
using IntegrationTesting.Tests.Application.DtoReturns.GetDtoReturns;
using IntegrationTesting.Tests.Application.DtoReturns.UpdateDtoReturn;
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
    public class DtoReturnsController : ControllerBase
    {
        private readonly ISender _mediator;

        public DtoReturnsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/dto-return")]
        [ProducesResponseType(typeof(DtoReturnDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DtoReturnDto>> CreateDtoReturn(
            [FromBody] CreateDtoReturnCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No DtoReturnDto could be found with the provided parameters.</response>
        [HttpDelete("api/dto-return/{id}")]
        [ProducesResponseType(typeof(DtoReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DtoReturnDto>> DeleteDtoReturn(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new DeleteDtoReturnCommand(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No DtoReturnDto could be found with the provided parameters.</response>
        [HttpPut("api/dto-return/{id}")]
        [ProducesResponseType(typeof(DtoReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DtoReturnDto>> UpdateDtoReturn(
            [FromRoute] Guid id,
            [FromBody] UpdateDtoReturnCommand command,
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

            var result = await _mediator.Send(command, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified DtoReturnDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No DtoReturnDto could be found with the provided parameters.</response>
        [HttpGet("api/dto-return/{id}")]
        [ProducesResponseType(typeof(DtoReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DtoReturnDto>> GetDtoReturnById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDtoReturnByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;DtoReturnDto&gt;.</response>
        [HttpGet("api/dto-return")]
        [ProducesResponseType(typeof(List<DtoReturnDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<DtoReturnDto>>> GetDtoReturns(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDtoReturnsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
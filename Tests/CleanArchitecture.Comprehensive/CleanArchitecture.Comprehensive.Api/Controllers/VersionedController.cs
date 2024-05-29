using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using CleanArchitecture.Comprehensive.Application.Versioned.TestCommandV1;
using CleanArchitecture.Comprehensive.Application.Versioned.TestCommandV2;
using CleanArchitecture.Comprehensive.Application.Versioned.TestQueryV1;
using CleanArchitecture.Comprehensive.Application.Versioned.TestQueryV2;
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
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class VersionedController : ControllerBase
    {
        private readonly ISender _mediator;

        public VersionedController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/v{version:apiVersion}/versioned/test-command")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> TestCommandV1(
            [FromBody] TestCommandV1 command,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// Command comment
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/v{version:apiVersion}/versioned/test-command")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult> TestCommandV2(
            [FromBody] TestCommandV2 command,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified int.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpGet("api/v{version:apiVersion}/versioned/test-query")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<int>> TestQueryV1(
            [FromQuery] string value,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new TestQueryV1(value: value), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified int.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpGet("api/v{version:apiVersion}/versioned/test-query")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<int>> TestQueryV2(
            [FromQuery] string value,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new TestQueryV2(value: value), cancellationToken);
            return Ok(result);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.Serilog.Application.Testing.PerformLargeObjectLogging;
using Standard.AspNetCore.Serilog.Application.Testing.PerformSmallObjectLogging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Standard.AspNetCore.Serilog.Api.Controllers
{
    [ApiController]
    public class TestingController : ControllerBase
    {
        private readonly ISender _mediator;

        public TestingController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        [HttpPut("api/testing/perform-large-object-logging")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PerformLargeObjectLogging(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new PerformLargeObjectLogging(), cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        [HttpPut("api/testing/perform-small-object-logging")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PerformSmallObjectLogging(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new PerformSmallObjectLogging(), cancellationToken);
            return NoContent();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.RequestSuffixCommandsWithType.MyCommandRequest;
using CleanArchitecture.Comprehensive.Application.RequestSuffixCommandsWithType.MyRequest;
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
    public class RequestSuffixCommandsWithTypeController : ControllerBase
    {
        private readonly ISender _mediator;

        public RequestSuffixCommandsWithTypeController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        [HttpPost("api/request-suffix-commands-with-type/my")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> My(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new MyCommandRequest(), cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        [HttpPost("api/request-suffix-commands-with-type/my-request")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> MyRequest(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new MyRequestCommand(), cancellationToken);
            return Created(string.Empty, null);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.RequestSuffixQueriesWithType.MyQueryRequest;
using CleanArchitecture.Comprehensive.Application.RequestSuffixQueriesWithType.MyRequest;
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
    public class RequestSuffixQueriesWithTypeController : ControllerBase
    {
        private readonly ISender _mediator;

        public RequestSuffixQueriesWithTypeController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified int.</response>
        [HttpGet("api/request-suffix-queries-with-type/my")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> My(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new MyQueryRequest(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified int.</response>
        [HttpGet("api/request-suffix-queries-with-type/my-request")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> MyRequest(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new MyRequestQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
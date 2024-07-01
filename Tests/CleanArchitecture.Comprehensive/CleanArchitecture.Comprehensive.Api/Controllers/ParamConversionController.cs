using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.ParamConversion.CheckTypeConversionsOnProxy;
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
    public class ParamConversionController : ControllerBase
    {
        private readonly ISender _mediator;

        public ParamConversionController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified bool.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpGet("api/param-conversion/check-type-conversions-on-proxy")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> CheckTypeConversionsOnProxy(
            [FromQuery] DateTime from,
            [FromQuery] DateTime? to,
            [FromQuery] Guid id,
            [FromQuery] decimal value,
            [FromQuery] TimeSpan time,
            [FromQuery] bool active,
            [FromQuery] DateOnly justDate,
            [FromQuery] DateTimeOffset otherDate,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CheckTypeConversionsOnProxy(from: from, to: to, id: id, value: value, time: time, active: active, justDate: justDate, otherDate: otherDate), cancellationToken);
            return Ok(result);
        }
    }
}
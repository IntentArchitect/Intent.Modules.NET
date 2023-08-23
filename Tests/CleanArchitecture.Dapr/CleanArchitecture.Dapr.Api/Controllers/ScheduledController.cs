using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Application.Scheduled.Daily;
using CleanArchitecture.Dapr.Application.Scheduled.Hourly;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CleanArchitecture.Dapr.Api.Controllers
{
    [ApiController]
    public class ScheduledController : ControllerBase
    {
        private readonly ISender _mediator;

        public ScheduledController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        [HttpPost("api/scheduled/daily")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Daily(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DailyCommand(), cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        [HttpPost("api/scheduled/hourly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Hourly(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new HourlyCommand(), cancellationToken);
            return Created(string.Empty, null);
        }
    }
}
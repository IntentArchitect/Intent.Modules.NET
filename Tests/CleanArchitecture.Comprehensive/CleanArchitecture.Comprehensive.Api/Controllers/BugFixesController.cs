using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.BugFixes;
using CleanArchitecture.Comprehensive.Application.BugFixes.GetTaskName;
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
    public class BugFixesController : ControllerBase
    {
        private readonly ISender _mediator;

        public BugFixesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified TaskNameDto.</response>
        [HttpGet("api/bug-fixes")]
        [ProducesResponseType(typeof(TaskNameDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TaskNameDto>> GetTaskName(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetTaskNameQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
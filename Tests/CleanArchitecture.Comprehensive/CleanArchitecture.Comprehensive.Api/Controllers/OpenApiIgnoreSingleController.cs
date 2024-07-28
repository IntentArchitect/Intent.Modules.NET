using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.OpenApiIgnoreSingle.OperationA;
using CleanArchitecture.Comprehensive.Application.OpenApiIgnoreSingle.OperationB;
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
    public class OpenApiIgnoreSingleController : ControllerBase
    {
        private readonly ISender _mediator;

        public OpenApiIgnoreSingleController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        [HttpPut("api/open-api-ignore-single/operation-a", Name = "OpenApiIgnoreSingle.OperationA")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> OperationA(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new OperationA(), cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        [HttpPut("api/open-api-ignore-single/operation-b")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> OperationB(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new OperationB(), cancellationToken);
            return NoContent();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Application.Concurrency.UpdateEntityAfterEtagWasChangedByPreviousOperationTest;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Api.Controllers
{
    [ApiController]
    public class ConcurrencyController : ControllerBase
    {
        private readonly ISender _mediator;

        public ConcurrencyController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        [HttpPut("api/concurrency")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEntityAfterEtagWasChangedByPreviousOperationTest(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new UpdateEntityAfterEtagWasChangedByPreviousOperationTest(), cancellationToken);
            return NoContent();
        }
    }
}
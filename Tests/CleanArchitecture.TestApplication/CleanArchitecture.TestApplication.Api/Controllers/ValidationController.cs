using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Application.Validation;
using CleanArchitecture.TestApplication.Application.Validation.ResultValidations;
using CleanArchitecture.TestApplication.Application.Validation.Validated;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Api.Controllers
{
    [ApiController]
    public class ValidationController : ControllerBase
    {
        private readonly ISender _mediator;

        public ValidationController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("api/validation/validated")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Validated(
            [FromBody] ValidatedCommand command,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified ValidatedResultDto.</response>
        /// <response code="404">Can't find an ValidatedResultDto with the parameters provided.</response>
        [HttpGet("api/validation/result-validations")]
        [ProducesResponseType(typeof(ValidatedResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ValidatedResultDto>> ResultValidations(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new ResultValidationsQuery(), cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }
    }
}
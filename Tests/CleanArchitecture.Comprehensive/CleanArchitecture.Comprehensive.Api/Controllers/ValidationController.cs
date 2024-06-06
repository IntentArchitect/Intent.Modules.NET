using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Validation;
using CleanArchitecture.Comprehensive.Application.Validation.InboundValidation;
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
        [HttpPut("api/validation/inbound-validation")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> InboundValidation(
            [FromBody] InboundValidationCommand command,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified DummyResultDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No DummyResultDto could be found with the provided parameters.</response>
        [HttpGet("api/validation/inbound-validation")]
        [ProducesResponseType(typeof(DummyResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DummyResultDto>> InboundValidation(
            [FromQuery] string rangeStr,
            [FromQuery] string minStr,
            [FromQuery] string maxStr,
            [FromQuery] int rangeInt,
            [FromQuery] int minInt,
            [FromQuery] int maxInt,
            [FromQuery] string isRequired,
            [FromQuery] string isRequiredEmpty,
            [FromQuery] decimal decimalRange,
            [FromQuery] decimal decimalMin,
            [FromQuery] decimal decimalMax,
            [FromQuery] string? stringOption,
            [FromQuery] string? stringOptionNonEmpty,
            [FromQuery] EnumDescriptions myEnum,
            [FromQuery] string regexField,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new InboundValidationQuery(rangeStr: rangeStr, minStr: minStr, maxStr: maxStr, rangeInt: rangeInt, minInt: minInt, maxInt: maxInt, isRequired: isRequired, isRequiredEmpty: isRequiredEmpty, decimalRange: decimalRange, decimalMin: decimalMin, decimalMax: decimalMax, stringOption: stringOption, stringOptionNonEmpty: stringOptionNonEmpty, myEnum: myEnum, regexField: regexField), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
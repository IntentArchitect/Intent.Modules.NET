using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Api.Controllers.ResponseTypes;
using CleanArchitecture.Comprehensive.Application.NullableRouteParameters;
using CleanArchitecture.Comprehensive.Application.NullableRouteParameters.NullableRouteParameter1;
using CleanArchitecture.Comprehensive.Application.NullableRouteParameters.NullableRouteParameter2;
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
    public class NullableRouteParametersController : ControllerBase
    {
        private readonly ISender _mediator;

        public NullableRouteParametersController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("api/nullable-route-parameters-command/{nullableString?}/{nullableInt?}/{nullableEnum?}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> NullableRouteParameter1(
            [FromRoute] string? nullableString,
            [FromRoute] int? nullableInt,
            [FromRoute] NullableRouteParameterEnum? nullableEnum,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new NullableRouteParameter1Command(nullableString: nullableString, nullableInt: nullableInt, nullableEnum: nullableEnum), cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified int.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpGet("api/nullable-route-parameters-query/{nullableString?}/{nullableInt?}/{nullableEnum?}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<int>>> NullableRouteParameter2(
            [FromRoute] string? nullableString,
            [FromRoute] int? nullableInt,
            [FromRoute] NullableRouteParameterEnum? nullableEnum,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new NullableRouteParameter2Query(nullableString: nullableString, nullableInt: nullableInt, nullableEnum: nullableEnum), cancellationToken);
            return Ok(new JsonResponse<int>(result));
        }
    }
}
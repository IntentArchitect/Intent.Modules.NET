using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.TestNullablities;
using CleanArchitecture.Comprehensive.Application.TestNullablities.GetTestNullabilityWithNullReturn;
using CleanArchitecture.Comprehensive.Application.TestNullablities.GetTestNullablityById;
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
    public class TestNullablitiesController : ControllerBase
    {
        private readonly ISender _mediator;

        public TestNullablitiesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified TestNullablityDto?.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/test-nullablity-with-null-return/{id}")]
        [ProducesResponseType(typeof(TestNullablityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TestNullablityDto?>> GetTestNullabilityWithNullReturn(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetTestNullabilityWithNullReturn(id: id), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified TestNullablityDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No TestNullablityDto could be found with the provided parameters.</response>
        [HttpGet("api/test-nullablity/{id}")]
        [ProducesResponseType(typeof(TestNullablityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TestNullablityDto>> GetTestNullablityById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetTestNullablityByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
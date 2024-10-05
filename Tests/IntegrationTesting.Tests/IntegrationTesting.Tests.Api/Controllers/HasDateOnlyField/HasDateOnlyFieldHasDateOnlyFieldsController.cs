using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Api.Controllers.ResponseTypes;
using IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields;
using IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.CreateHasDateOnlyField;
using IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.DeleteHasDateOnlyField;
using IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.GetHasDateOnlyFieldById;
using IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.GetHasDateOnlyFields;
using IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.UpdateHasDateOnlyField;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace IntegrationTesting.Tests.Api.Controllers.HasDateOnlyField
{
    [ApiController]
    public class HasDateOnlyFieldHasDateOnlyFieldsController : ControllerBase
    {
        private readonly ISender _mediator;

        public HasDateOnlyFieldHasDateOnlyFieldsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/has-date-only-field/has-date-only-fields")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateHasDateOnlyField(
            [FromBody] CreateHasDateOnlyFieldCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetHasDateOnlyFieldById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/has-date-only-field/has-date-only-fields/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteHasDateOnlyField(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteHasDateOnlyFieldCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/has-date-only-field/has-date-only-fields/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateHasDateOnlyField(
            [FromRoute] Guid id,
            [FromBody] UpdateHasDateOnlyFieldCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == default)
            {
                command.Id = id;
            }

            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified HasDateOnlyFieldDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No HasDateOnlyFieldDto could be found with the provided parameters.</response>
        [HttpGet("api/has-date-only-field/has-date-only-fields/{id}")]
        [ProducesResponseType(typeof(HasDateOnlyFieldDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HasDateOnlyFieldDto>> GetHasDateOnlyFieldById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetHasDateOnlyFieldByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;HasDateOnlyFieldDto&gt;.</response>
        [HttpGet("api/has-date-only-field/has-date-only-fields")]
        [ProducesResponseType(typeof(List<HasDateOnlyFieldDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<HasDateOnlyFieldDto>>> GetHasDateOnlyFields(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetHasDateOnlyFieldsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
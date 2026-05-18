using System.Net.Mime;
using AdvancedMapping.Repositories.Mapperly.Tests.Api.Controllers.ResponseTypes;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.CreateCustomerSegments;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.GetCustomerSegments;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.GetCustomerSegmentsById;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.UpdateCustomerSegments;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Api.Controllers
{
    [ApiController]
    public class CustomerSegmentsController : ControllerBase
    {
        private readonly ISender _mediator;

        public CustomerSegmentsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/customer-segments")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateCustomerSegments(
            [FromBody] CreateCustomerSegmentsCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetCustomerSegmentsById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/customer-segments/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCustomerSegments(
            [FromRoute] Guid id,
            [FromBody] UpdateCustomerSegmentsCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == Guid.Empty)
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
        /// <response code="200">Returns the specified CustomerSegmentsDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No CustomerSegmentsDto could be found with the provided parameters.</response>
        [HttpGet("api/customer-segments/{id}")]
        [ProducesResponseType(typeof(CustomerSegmentsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerSegmentsDto>> GetCustomerSegmentsById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomerSegmentsByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;CustomerSegmentsDto&gt;.</response>
        [HttpGet("api/customer-segments")]
        [ProducesResponseType(typeof(List<CustomerSegmentsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CustomerSegmentsDto>>> GetCustomerSegments(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomerSegmentsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
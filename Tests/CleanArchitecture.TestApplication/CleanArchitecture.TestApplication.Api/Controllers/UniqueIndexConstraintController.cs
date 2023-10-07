using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Api.Controllers.ResponseTypes;
using CleanArchitecture.TestApplication.Application.UniqueIndexConstraint;
using CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.CreateAggregateWithUniqueConstraintIndexElement;
using CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.CreateAggregateWithUniqueConstraintIndexStereotype;
using CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.DeleteAggregateWithUniqueConstraintIndexElement;
using CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.DeleteAggregateWithUniqueConstraintIndexStereotype;
using CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.GetAggregateWithUniqueConstraintIndexElementById;
using CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.GetAggregateWithUniqueConstraintIndexElements;
using CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.GetAggregateWithUniqueConstraintIndexStereotypeById;
using CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.GetAggregateWithUniqueConstraintIndexStereotypes;
using CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.UpdateAggregateWithUniqueConstraintIndexElement;
using CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.UpdateAggregateWithUniqueConstraintIndexStereotype;
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
    public class UniqueIndexConstraintController : ControllerBase
    {
        private readonly ISender _mediator;

        public UniqueIndexConstraintController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/unique-index-constraint-element")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateAggregateWithUniqueConstraintIndexElement(
            [FromBody] CreateAggregateWithUniqueConstraintIndexElementCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetAggregateWithUniqueConstraintIndexElementById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/unique-index-constraint-stereotype")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateAggregateWithUniqueConstraintIndexStereotype(
            [FromBody] CreateAggregateWithUniqueConstraintIndexStereotypeCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetAggregateWithUniqueConstraintIndexElementById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/unique-index-constraint-element/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAggregateWithUniqueConstraintIndexElement(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteAggregateWithUniqueConstraintIndexElementCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/unique-index-constraint-stereotype/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAggregateWithUniqueConstraintIndexStereotype(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteAggregateWithUniqueConstraintIndexStereotypeCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/unique-index-constraint-element/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateAggregateWithUniqueConstraintIndexElement(
            [FromRoute] Guid id,
            [FromBody] UpdateAggregateWithUniqueConstraintIndexElementCommand command,
            CancellationToken cancellationToken = default)
        {
            command.SetId(id);
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/unique-index-constraint-stereotype/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateAggregateWithUniqueConstraintIndexStereotype(
            [FromRoute] Guid id,
            [FromBody] UpdateAggregateWithUniqueConstraintIndexStereotypeCommand command,
            CancellationToken cancellationToken = default)
        {
            command.SetId(id);
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified AggregateWithUniqueConstraintIndexElementDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No AggregateWithUniqueConstraintIndexElementDto could be found with the provided parameters.</response>
        [HttpGet("api/unique-index-constraint-element/{id}")]
        [ProducesResponseType(typeof(AggregateWithUniqueConstraintIndexElementDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AggregateWithUniqueConstraintIndexElementDto>> GetAggregateWithUniqueConstraintIndexElementById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAggregateWithUniqueConstraintIndexElementByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;AggregateWithUniqueConstraintIndexElementDto&gt;.</response>
        [HttpGet("api/unique-index-constraint-element")]
        [ProducesResponseType(typeof(List<AggregateWithUniqueConstraintIndexElementDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AggregateWithUniqueConstraintIndexElementDto>>> GetAggregateWithUniqueConstraintIndexElements(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAggregateWithUniqueConstraintIndexElementsQuery(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified AggregateWithUniqueConstraintIndexStereotypeDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No AggregateWithUniqueConstraintIndexStereotypeDto could be found with the provided parameters.</response>
        [HttpGet("api/unique-index-constraint-stereotype/{id}")]
        [ProducesResponseType(typeof(AggregateWithUniqueConstraintIndexStereotypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AggregateWithUniqueConstraintIndexStereotypeDto>> GetAggregateWithUniqueConstraintIndexStereotypeById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAggregateWithUniqueConstraintIndexStereotypeByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;AggregateWithUniqueConstraintIndexStereotypeDto&gt;.</response>
        [HttpGet("api/unique-index-constraint-stereotype")]
        [ProducesResponseType(typeof(List<AggregateWithUniqueConstraintIndexStereotypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AggregateWithUniqueConstraintIndexStereotypeDto>>> GetAggregateWithUniqueConstraintIndexStereotypes(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAggregateWithUniqueConstraintIndexStereotypesQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
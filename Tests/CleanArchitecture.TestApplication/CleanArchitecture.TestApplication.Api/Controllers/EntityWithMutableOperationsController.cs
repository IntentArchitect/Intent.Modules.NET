using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Api.Controllers.ResponseTypes;
using CleanArchitecture.TestApplication.Application.EntityWithMutableOperations;
using CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.CreateEntityWithMutableOperation;
using CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.DeleteEntityWithMutableOperation;
using CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.GetEntityWithMutableOperationById;
using CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.GetEntityWithMutableOperations;
using CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.UpdateEntityWithMutableOperation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Api.Controllers
{
    [ApiController]
    public class EntityWithMutableOperationsController : ControllerBase
    {
        private readonly ISender _mediator;

        public EntityWithMutableOperationsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/entity-with-mutable-operations")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateEntityWithMutableOperation(
            [FromBody] CreateEntityWithMutableOperationCommand command,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpDelete("api/entity-with-mutable-operations/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteEntityWithMutableOperation(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteEntityWithMutableOperationCommand { Id = id }, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("api/entity-with-mutable-operations/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEntityWithMutableOperation(
            [FromRoute] Guid id,
            [FromBody] UpdateEntityWithMutableOperationCommand command,
            CancellationToken cancellationToken)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified EntityWithMutableOperationDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an EntityWithMutableOperationDto with the parameters provided.</response>
        [HttpGet("api/entity-with-mutable-operations/{id}")]
        [ProducesResponseType(typeof(EntityWithMutableOperationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EntityWithMutableOperationDto>> GetEntityWithMutableOperationById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetEntityWithMutableOperationByIdQuery { Id = id }, cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;EntityWithMutableOperationDto&gt;.</response>
        [HttpGet("api/entity-with-mutable-operations")]
        [ProducesResponseType(typeof(List<EntityWithMutableOperationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EntityWithMutableOperationDto>>> GetEntityWithMutableOperations(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetEntityWithMutableOperationsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
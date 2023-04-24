using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Api.Controllers.ResponseTypes;
using CleanArchitecture.TestApplication.Application.EntityWithCtors;
using CleanArchitecture.TestApplication.Application.EntityWithCtors.CreateEntityWithCtor;
using CleanArchitecture.TestApplication.Application.EntityWithCtors.DeleteEntityWithCtor;
using CleanArchitecture.TestApplication.Application.EntityWithCtors.GetEntityWithCtorById;
using CleanArchitecture.TestApplication.Application.EntityWithCtors.GetEntityWithCtors;
using CleanArchitecture.TestApplication.Application.EntityWithCtors.UpdateEntityWithCtor;
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
    public class EntityWithCtorsController : ControllerBase
    {
        private readonly ISender _mediator;

        public EntityWithCtorsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/entity-with-ctors")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> CreateEntityWithCtor(
            [FromBody] CreateEntityWithCtorCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetEntityWithCtorById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpDelete("api/entity-with-ctors/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteEntityWithCtor([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteEntityWithCtorCommand { Id = id }, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("api/entity-with-ctors/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEntityWithCtor(
            [FromRoute] Guid id,
            [FromBody] UpdateEntityWithCtorCommand command,
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
        /// <response code="200">Returns the specified EntityWithCtorDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an EntityWithCtorDto with the parameters provided.</response>
        [HttpGet("api/entity-with-ctors/{id}")]
        [ProducesResponseType(typeof(EntityWithCtorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EntityWithCtorDto>> GetEntityWithCtorById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetEntityWithCtorByIdQuery { Id = id }, cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;EntityWithCtorDto&gt;.</response>
        [HttpGet("api/entity-with-ctors")]
        [ProducesResponseType(typeof(List<EntityWithCtorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EntityWithCtorDto>>> GetEntityWithCtors(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetEntityWithCtorsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
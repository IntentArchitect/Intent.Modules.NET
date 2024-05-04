using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Api.Controllers.ResponseTypes;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.CreateEntityDefault;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.DeleteEntityDefault;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.GetEntityDefaultById;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.GetEntityDefaults;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.UpdateEntityDefault;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Api.Controllers
{
    [ApiController]
    public class EntityDefaultsController : ControllerBase
    {
        private readonly ISender _mediator;

        public EntityDefaultsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/entity-default")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateEntityDefault(
            [FromBody] CreateEntityDefaultCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetEntityDefaultById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/entity-default/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteEntityDefault(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteEntityDefaultCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/entity-default/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEntityDefault(
            [FromRoute] Guid id,
            [FromBody] UpdateEntityDefaultCommand command,
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
        /// <response code="200">Returns the specified EntityDefaultDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No EntityDefaultDto could be found with the provided parameters.</response>
        [HttpGet("api/entity-default/{id}")]
        [ProducesResponseType(typeof(EntityDefaultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EntityDefaultDto>> GetEntityDefaultById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetEntityDefaultByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;EntityDefaultDto&gt;.</response>
        [HttpGet("api/entity-default")]
        [ProducesResponseType(typeof(List<EntityDefaultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EntityDefaultDto>>> GetEntityDefaults(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetEntityDefaultsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
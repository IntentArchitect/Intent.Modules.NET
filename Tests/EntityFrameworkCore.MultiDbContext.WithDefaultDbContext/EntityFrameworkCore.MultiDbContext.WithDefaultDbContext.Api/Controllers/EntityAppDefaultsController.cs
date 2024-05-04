using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Api.Controllers.ResponseTypes;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.CreateEntityAppDefault;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.DeleteEntityAppDefault;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.GetEntityAppDefaultById;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.GetEntityAppDefaults;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.UpdateEntityAppDefault;
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
    public class EntityAppDefaultsController : ControllerBase
    {
        private readonly ISender _mediator;

        public EntityAppDefaultsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/entity-app-default")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateEntityAppDefault(
            [FromBody] CreateEntityAppDefaultCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetEntityAppDefaultById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/entity-app-default/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteEntityAppDefault(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteEntityAppDefaultCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/entity-app-default/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEntityAppDefault(
            [FromRoute] Guid id,
            [FromBody] UpdateEntityAppDefaultCommand command,
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
        /// <response code="200">Returns the specified EntityAppDefaultDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No EntityAppDefaultDto could be found with the provided parameters.</response>
        [HttpGet("api/entity-app-default/{id}")]
        [ProducesResponseType(typeof(EntityAppDefaultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EntityAppDefaultDto>> GetEntityAppDefaultById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetEntityAppDefaultByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;EntityAppDefaultDto&gt;.</response>
        [HttpGet("api/entity-app-default")]
        [ProducesResponseType(typeof(List<EntityAppDefaultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EntityAppDefaultDto>>> GetEntityAppDefaults(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetEntityAppDefaultsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
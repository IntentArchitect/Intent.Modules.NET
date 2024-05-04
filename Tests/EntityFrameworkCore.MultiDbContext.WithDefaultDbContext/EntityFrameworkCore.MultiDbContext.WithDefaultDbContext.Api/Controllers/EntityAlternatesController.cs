using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Api.Controllers.ResponseTypes;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.CreateEntityAlternate;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.DeleteEntityAlternate;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.GetEntityAlternateById;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.GetEntityAlternates;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.UpdateEntityAlternate;
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
    public class EntityAlternatesController : ControllerBase
    {
        private readonly ISender _mediator;

        public EntityAlternatesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/entity-alternate")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateEntityAlternate(
            [FromBody] CreateEntityAlternateCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetEntityAlternateById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/entity-alternate/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteEntityAlternate(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteEntityAlternateCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/entity-alternate/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEntityAlternate(
            [FromRoute] Guid id,
            [FromBody] UpdateEntityAlternateCommand command,
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
        /// <response code="200">Returns the specified EntityAlternateDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No EntityAlternateDto could be found with the provided parameters.</response>
        [HttpGet("api/entity-alternate/{id}")]
        [ProducesResponseType(typeof(EntityAlternateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EntityAlternateDto>> GetEntityAlternateById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetEntityAlternateByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;EntityAlternateDto&gt;.</response>
        [HttpGet("api/entity-alternate")]
        [ProducesResponseType(typeof(List<EntityAlternateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EntityAlternateDto>>> GetEntityAlternates(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetEntityAlternatesQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
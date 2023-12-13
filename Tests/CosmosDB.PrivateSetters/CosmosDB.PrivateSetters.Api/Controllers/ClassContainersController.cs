using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.PrivateSetters.Api.Controllers.ResponseTypes;
using CosmosDB.PrivateSetters.Application.ClassContainers;
using CosmosDB.PrivateSetters.Application.ClassContainers.CreateClassContainer;
using CosmosDB.PrivateSetters.Application.ClassContainers.DeleteClassContainer;
using CosmosDB.PrivateSetters.Application.ClassContainers.GetClassContainerById;
using CosmosDB.PrivateSetters.Application.ClassContainers.GetClassContainers;
using CosmosDB.PrivateSetters.Application.ClassContainers.UpdateClassContainer;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Api.Controllers
{
    [ApiController]
    public class ClassContainersController : ControllerBase
    {
        private readonly ISender _mediator;

        public ClassContainersController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/class-containers")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateClassContainer(
            [FromBody] CreateClassContainerCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/class-containers/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteClassContainer(
            [FromRoute] string id,
            [FromQuery] string classPartitionKey,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteClassContainerCommand(id: id, classPartitionKey: classPartitionKey), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/class-containers/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateClassContainer(
            [FromRoute] string id,
            [FromBody] UpdateClassContainerCommand command,
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
        /// <response code="200">Returns the specified ClassContainerDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No ClassContainerDto could be found with the provided parameters.</response>
        [HttpGet("api/class-containers/{id}")]
        [ProducesResponseType(typeof(ClassContainerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClassContainerDto>> GetClassContainerById(
            [FromRoute] string id,
            [FromQuery] string classPartitionKey,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetClassContainerByIdQuery(id: id, classPartitionKey: classPartitionKey), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ClassContainerDto&gt;.</response>
        [HttpGet("api/class-containers")]
        [ProducesResponseType(typeof(List<ClassContainerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ClassContainerDto>>> GetClassContainers(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetClassContainersQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Api.Controllers.ResponseTypes;
using AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.CreateTaskItem;
using AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.CreateUserWithTaskItem;
using AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.CreateUserWithTaskItemContract;
using AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.DeleteTaskItem;
using AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.GetTaskItemById;
using AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.GetTaskItems;
using AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.UpdateTaskItem;
using AdvancedMappingCrud.Repositories.Tests.Application.Users;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Api.Controllers
{
    [ApiController]
    public class OperationMappingController : ControllerBase
    {
        private readonly ISender _mediator;

        public OperationMappingController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("api/users/{userId}/tasklists/{taskListId}/taskItems")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateTaskItem(
            [FromRoute] Guid userId,
            [FromRoute] Guid taskListId,
            [FromBody] CreateTaskItemCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.UserId == Guid.Empty)
            {
                command.UserId = userId;
            }

            if (command.TaskListId == Guid.Empty)
            {
                command.TaskListId = taskListId;
            }

            if (userId != command.UserId)
            {
                return BadRequest();
            }

            if (taskListId != command.TaskListId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/operation-mapping/with-task-item")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateUserWithTaskItem(
            [FromBody] CreateUserWithTaskItemCommand command,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/operation-mapping/with-task-item-contract")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateUserWithTaskItemContract(
            [FromBody] CreateUserWithTaskItemContractCommand command,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/users/{userId}/tasklists/{taskListId}/taskitems/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteTaskItem(
            [FromRoute] Guid userId,
            [FromRoute] Guid taskListId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteTaskItemCommand(userId: userId, taskListId: taskListId, id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/users/{userId}/tasklists/{taskListId}/taskitems/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateTaskItem(
            [FromRoute] Guid userId,
            [FromRoute] Guid taskListId,
            [FromRoute] Guid id,
            [FromBody] UpdateTaskItemCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.UserId == Guid.Empty)
            {
                command.UserId = userId;
            }

            if (command.TaskListId == Guid.Empty)
            {
                command.TaskListId = taskListId;
            }

            if (command.Id == Guid.Empty)
            {
                command.Id = id;
            }

            if (userId != command.UserId)
            {
                return BadRequest();
            }

            if (taskListId != command.TaskListId)
            {
                return BadRequest();
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
        /// <response code="200">Returns the specified TaskItemDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No TaskItemDto could be found with the provided parameters.</response>
        [HttpGet("api/users/{userId}/tasklists/{taskListId}/taskitems/{id}")]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TaskItemDto>> GetTaskItemById(
            [FromRoute] Guid userId,
            [FromRoute] Guid taskListId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetTaskItemByIdQuery(userId: userId, taskListId: taskListId, id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;TaskItemDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No List&lt;TaskItemDto&gt; could be found with the provided parameters.</response>
        [HttpGet("api/users/{userId}/tasklists/{taskListId}/taskItems")]
        [ProducesResponseType(typeof(List<TaskItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<TaskItemDto>>> GetTaskItems(
            [FromRoute] Guid userId,
            [FromRoute] Guid taskListId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetTaskItemsQuery(userId: userId, taskListId: taskListId), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
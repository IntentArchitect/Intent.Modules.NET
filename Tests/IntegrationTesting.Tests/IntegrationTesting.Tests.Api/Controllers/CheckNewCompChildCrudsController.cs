using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Api.Controllers.ResponseTypes;
using IntegrationTesting.Tests.Application.CheckNewCompChildCruds;
using IntegrationTesting.Tests.Application.CheckNewCompChildCruds.CreateCheckNewCompChildCrud;
using IntegrationTesting.Tests.Application.CheckNewCompChildCruds.CreateCNCCChild;
using IntegrationTesting.Tests.Application.CheckNewCompChildCruds.DeleteCheckNewCompChildCrud;
using IntegrationTesting.Tests.Application.CheckNewCompChildCruds.DeleteCNCCChild;
using IntegrationTesting.Tests.Application.CheckNewCompChildCruds.GetCheckNewCompChildCrudById;
using IntegrationTesting.Tests.Application.CheckNewCompChildCruds.GetCheckNewCompChildCruds;
using IntegrationTesting.Tests.Application.CheckNewCompChildCruds.GetCNCCChildById;
using IntegrationTesting.Tests.Application.CheckNewCompChildCruds.GetCNCCChildren;
using IntegrationTesting.Tests.Application.CheckNewCompChildCruds.UpdateCheckNewCompChildCrud;
using IntegrationTesting.Tests.Application.CheckNewCompChildCruds.UpdateCNCCChild;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace IntegrationTesting.Tests.Api.Controllers
{
    [ApiController]
    public class CheckNewCompChildCrudsController : ControllerBase
    {
        private readonly ISender _mediator;

        public CheckNewCompChildCrudsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/check-new-comp-child-cruds")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateCheckNewCompChildCrud(
            [FromBody] CreateCheckNewCompChildCrudCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetCheckNewCompChildCrudById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("api/check-new-comp-child-cruds/{checkNewCompChildCrudId}/c-n-c-c-children")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateCNCCChild(
            [FromRoute] Guid checkNewCompChildCrudId,
            [FromBody] CreateCNCCChildCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.CheckNewCompChildCrudId == Guid.Empty)
            {
                command.CheckNewCompChildCrudId = checkNewCompChildCrudId;
            }

            if (checkNewCompChildCrudId != command.CheckNewCompChildCrudId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetCNCCChildById), new { checkNewCompChildCrudId = checkNewCompChildCrudId, id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/check-new-comp-child-cruds/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCheckNewCompChildCrud(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteCheckNewCompChildCrudCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/check-new-comp-child-cruds/{checkNewCompChildCrudId}/c-n-c-c-children/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCNCCChild(
            [FromRoute] Guid checkNewCompChildCrudId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteCNCCChildCommand(checkNewCompChildCrudId: checkNewCompChildCrudId, id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/check-new-comp-child-cruds/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCheckNewCompChildCrud(
            [FromRoute] Guid id,
            [FromBody] UpdateCheckNewCompChildCrudCommand command,
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
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/check-new-comp-child-cruds/{checkNewCompChildCrudId}/c-n-c-c-children/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCNCCChild(
            [FromRoute] Guid checkNewCompChildCrudId,
            [FromRoute] Guid id,
            [FromBody] UpdateCNCCChildCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.CheckNewCompChildCrudId == Guid.Empty)
            {
                command.CheckNewCompChildCrudId = checkNewCompChildCrudId;
            }

            if (command.Id == Guid.Empty)
            {
                command.Id = id;
            }

            if (checkNewCompChildCrudId != command.CheckNewCompChildCrudId)
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
        /// <response code="200">Returns the specified CheckNewCompChildCrudDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No CheckNewCompChildCrudDto could be found with the provided parameters.</response>
        [HttpGet("api/check-new-comp-child-cruds/{id}")]
        [ProducesResponseType(typeof(CheckNewCompChildCrudDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CheckNewCompChildCrudDto>> GetCheckNewCompChildCrudById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCheckNewCompChildCrudByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;CheckNewCompChildCrudDto&gt;.</response>
        [HttpGet("api/check-new-comp-child-cruds")]
        [ProducesResponseType(typeof(List<CheckNewCompChildCrudDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CheckNewCompChildCrudDto>>> GetCheckNewCompChildCruds(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCheckNewCompChildCrudsQuery(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified CNCCChildDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No CNCCChildDto could be found with the provided parameters.</response>
        [HttpGet("api/check-new-comp-child-cruds/{checkNewCompChildCrudId}/c-n-c-c-children/{id}")]
        [ProducesResponseType(typeof(CNCCChildDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CNCCChildDto>> GetCNCCChildById(
            [FromRoute] Guid checkNewCompChildCrudId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCNCCChildByIdQuery(checkNewCompChildCrudId: checkNewCompChildCrudId, id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;CNCCChildDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No List&lt;CNCCChildDto&gt; could be found with the provided parameters.</response>
        [HttpGet("api/check-new-comp-child-cruds/{checkNewCompChildCrudId}/c-n-c-c-children")]
        [ProducesResponseType(typeof(List<CNCCChildDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CNCCChildDto>>> GetCNCCChildren(
            [FromRoute] Guid checkNewCompChildCrudId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCNCCChildrenQuery(checkNewCompChildCrudId: checkNewCompChildCrudId), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Api.Controllers.ResponseTypes;
using AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren;
using AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.CreateParentWithAnemicChild;
using AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.DeleteParentWithAnemicChild;
using AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.GetParentWithAnemicChildById;
using AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.GetParentWithAnemicChildren;
using AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.UpdateParentWithAnemicChild;
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
    public class ParentWithAnemicChildrenController : ControllerBase
    {
        private readonly ISender _mediator;

        public ParentWithAnemicChildrenController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/parent-with-anemic-children")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateParentWithAnemicChild(
            [FromBody] CreateParentWithAnemicChildCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetParentWithAnemicChildById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/parent-with-anemic-children/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteParentWithAnemicChild(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteParentWithAnemicChildCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/parent-with-anemic-children/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateParentWithAnemicChild(
            [FromRoute] Guid id,
            [FromBody] UpdateParentWithAnemicChildCommand command,
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
        /// <response code="200">Returns the specified ParentWithAnemicChildDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No ParentWithAnemicChildDto could be found with the provided parameters.</response>
        [HttpGet("api/parent-with-anemic-children/{id}")]
        [ProducesResponseType(typeof(ParentWithAnemicChildDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ParentWithAnemicChildDto>> GetParentWithAnemicChildById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetParentWithAnemicChildByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ParentWithAnemicChildDto&gt;.</response>
        [HttpGet("api/parent-with-anemic-children")]
        [ProducesResponseType(typeof(List<ParentWithAnemicChildDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ParentWithAnemicChildDto>>> GetParentWithAnemicChildren(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetParentWithAnemicChildrenQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
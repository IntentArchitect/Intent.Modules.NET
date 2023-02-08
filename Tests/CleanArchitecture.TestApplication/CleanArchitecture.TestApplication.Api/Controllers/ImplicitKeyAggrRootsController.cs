using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Api.Controllers.ResponseTypes;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRoot;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRootImplicitKeyNestedComposition;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.DeleteImplicitKeyAggrRoot;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.DeleteImplicitKeyAggrRootImplicitKeyNestedComposition;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootById;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootImplicitKeyNestedCompositionById;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootImplicitKeyNestedCompositions;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRoot;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRootImplicitKeyNestedComposition;
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
    [Route("api/[controller]")]
    public class ImplicitKeyAggrRootsController : ControllerBase
    {
        private readonly ISender _mediator;

        public ImplicitKeyAggrRootsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Post([FromBody] CreateImplicitKeyAggrRootCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = result }, new { Id = result });
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified ImplicitKeyAggrRootDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an ImplicitKeyAggrRootDto with the parameters provided.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ImplicitKeyAggrRootDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ImplicitKeyAggrRootDto>> Get([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetImplicitKeyAggrRootByIdQuery { Id = id }, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ImplicitKeyAggrRootDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ImplicitKeyAggrRootDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ImplicitKeyAggrRootDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetImplicitKeyAggrRootsQuery(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Put([FromRoute] Guid id, [FromBody] UpdateImplicitKeyAggrRootCommand command, CancellationToken cancellationToken)
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
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete([FromRoute] Guid id, [FromQuery] DeleteImplicitKeyAggrRootCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("{implicitKeyAggrRootId}/ImplicitKeyNestedCompositions")]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> PostImplicitKeyNestedComposition([FromRoute] Guid implicitKeyAggrRootId, [FromBody] CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand command, CancellationToken cancellationToken)
        {
            if (implicitKeyAggrRootId != command.ImplicitKeyAggrRootId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = result }, new { Id = result });
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified ImplicitKeyAggrRootImplicitKeyNestedCompositionDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an ImplicitKeyAggrRootImplicitKeyNestedCompositionDto with the parameters provided.</response>
        [HttpGet("{implicitKeyAggrRootId}/ImplicitKeyNestedCompositions/{id}")]
        [ProducesResponseType(typeof(ImplicitKeyAggrRootImplicitKeyNestedCompositionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto>> GetImplicitKeyNestedComposition([FromRoute] Guid implicitKeyAggrRootId, [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQuery { ImplicitKeyAggrRootId = implicitKeyAggrRootId, Id = id }, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ImplicitKeyAggrRootImplicitKeyNestedCompositionDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("{implicitKeyAggrRootId}/ImplicitKeyNestedCompositions")]
        [ProducesResponseType(typeof(List<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto>>> GetAllImplicitKeyNestedCompositions([FromRoute] Guid implicitKeyAggrRootId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQuery { ImplicitKeyAggrRootId = implicitKeyAggrRootId }, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("{implicitKeyAggrRootId}/ImplicitKeyNestedCompositions/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PutImplicitKeyNestedComposition([FromRoute] Guid implicitKeyAggrRootId, [FromRoute] Guid id, [FromBody] UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand command, CancellationToken cancellationToken)
        {
            if (implicitKeyAggrRootId != command.ImplicitKeyAggrRootId)
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
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpDelete("{implicitKeyAggrRootId}/ImplicitKeyNestedCompositions/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteImplicitKeyNestedComposition([FromRoute] Guid implicitKeyAggrRootId, [FromRoute] Guid id, [FromQuery] DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand command, CancellationToken cancellationToken)
        {
            if (implicitKeyAggrRootId != command.ImplicitKeyAggrRootId)
            {
                return BadRequest();
            }
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return Ok();
        }
    }
}
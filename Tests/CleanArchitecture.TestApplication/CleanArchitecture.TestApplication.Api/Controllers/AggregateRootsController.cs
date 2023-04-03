using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Api.Controllers.ResponseTypes;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRoot;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Application.AggregateRoots.DeleteAggregateRoot;
using CleanArchitecture.TestApplication.Application.AggregateRoots.DeleteAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Application.AggregateRoots.GetAggregateRootById;
using CleanArchitecture.TestApplication.Application.AggregateRoots.GetAggregateRootCompositeManyBById;
using CleanArchitecture.TestApplication.Application.AggregateRoots.GetAggregateRootCompositeManyBS;
using CleanArchitecture.TestApplication.Application.AggregateRoots.GetAggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRoot;
using CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRootCompositeManyB;
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
    public class AggregateRootsController : ControllerBase
    {
        private readonly ISender _mediator;

        public AggregateRootsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Post([FromBody] CreateAggregateRootCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = result }, new { Id = result });
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified AggregateRootDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an AggregateRootDto with the parameters provided.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AggregateRootDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AggregateRootDto>> Get([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAggregateRootByIdQuery { Id = id }, cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;AggregateRootDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<AggregateRootDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AggregateRootDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAggregateRootsQuery(), cancellationToken);
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
        public async Task<ActionResult> Put([FromRoute] Guid id, [FromBody] UpdateAggregateRootCommand command, CancellationToken cancellationToken)
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
        public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteAggregateRootCommand { Id = id }, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("{aggregateRootId}/CompositeManyBS")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> PostCompositeManyB([FromRoute] Guid aggregateRootId, [FromBody] CreateAggregateRootCompositeManyBCommand command, CancellationToken cancellationToken)
        {
            if (aggregateRootId != command.AggregateRootId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = result }, new { Id = result });
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified AggregateRootCompositeManyBDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an AggregateRootCompositeManyBDto with the parameters provided.</response>
        [HttpGet("{aggregateRootId}/CompositeManyBS/{id}")]
        [ProducesResponseType(typeof(AggregateRootCompositeManyBDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AggregateRootCompositeManyBDto>> GetCompositeManyB([FromRoute] Guid aggregateRootId, [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAggregateRootCompositeManyBByIdQuery { AggregateRootId = aggregateRootId, Id = id }, cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;AggregateRootCompositeManyBDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("{aggregateRootId}/CompositeManyBS")]
        [ProducesResponseType(typeof(List<AggregateRootCompositeManyBDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AggregateRootCompositeManyBDto>>> GetAllCompositeManyBS([FromRoute] Guid aggregateRootId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAggregateRootCompositeManyBSQuery { AggregateRootId = aggregateRootId }, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("{aggregateRootId}/CompositeManyBS/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PutCompositeManyB([FromRoute] Guid aggregateRootId, [FromRoute] Guid id, [FromBody] UpdateAggregateRootCompositeManyBCommand command, CancellationToken cancellationToken)
        {
            if (aggregateRootId != command.AggregateRootId)
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
        [HttpDelete("{aggregateRootId}/CompositeManyBS/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCompositeManyB([FromRoute] Guid aggregateRootId, [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteAggregateRootCompositeManyBCommand { AggregateRootId = aggregateRootId, Id = id }, cancellationToken);
            return Ok();
        }
    }
}
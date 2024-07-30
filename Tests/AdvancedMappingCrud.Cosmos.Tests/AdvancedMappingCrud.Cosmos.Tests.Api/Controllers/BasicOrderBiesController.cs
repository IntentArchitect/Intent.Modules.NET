using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Api.Controllers.ResponseTypes;
using AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies;
using AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.CreateBasicOrderBy;
using AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.DeleteBasicOrderBy;
using AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.GetBasicOrderBy;
using AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.GetBasicOrderByById;
using AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.UpdateBasicOrderBy;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Api.Controllers
{
    [ApiController]
    public class BasicOrderBiesController : ControllerBase
    {
        private readonly ISender _mediator;

        public BasicOrderBiesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/basic-order-bies")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateBasicOrderBy(
            [FromBody] CreateBasicOrderByCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetBasicOrderByById), new { id = result }, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/basic-order-bies/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteBasicOrderBy(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteBasicOrderByCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/basic-order-bies/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateBasicOrderBy(
            [FromRoute] string id,
            [FromBody] UpdateBasicOrderByCommand command,
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
        /// <response code="200">Returns the specified BasicOrderByDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No BasicOrderByDto could be found with the provided parameters.</response>
        [HttpGet("api/basic-order-bies/{id}")]
        [ProducesResponseType(typeof(BasicOrderByDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BasicOrderByDto>> GetBasicOrderByById(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBasicOrderByByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;BasicOrderByDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/basic-order-bies")]
        [ProducesResponseType(typeof(PagedResult<BasicOrderByDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<BasicOrderByDto>>> GetBasicOrderBy(
            [FromQuery] int pageNo,
            [FromQuery] int pageSize,
            [FromQuery] string orderBy,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBasicOrderByQuery(pageNo: pageNo, pageSize: pageSize, orderBy: orderBy), cancellationToken);
            return Ok(result);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Api.Controllers.ResponseTypes;
using AdvancedMappingCrud.Repositories.Tests.Application.Basics;
using AdvancedMappingCrud.Repositories.Tests.Application.Basics.CreateBasic;
using AdvancedMappingCrud.Repositories.Tests.Application.Basics.DeleteBasic;
using AdvancedMappingCrud.Repositories.Tests.Application.Basics.GetBasicById;
using AdvancedMappingCrud.Repositories.Tests.Application.Basics.GetBasics;
using AdvancedMappingCrud.Repositories.Tests.Application.Basics.GetBasicsNullable;
using AdvancedMappingCrud.Repositories.Tests.Application.Basics.UpdateBasic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Pagination;
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
    public class BasicsController : ControllerBase
    {
        private readonly ISender _mediator;

        public BasicsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/basics")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateBasic(
            [FromBody] CreateBasicCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetBasicById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/basics/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteBasic([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteBasicCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/basics/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateBasic(
            [FromRoute] Guid id,
            [FromBody] UpdateBasicCommand command,
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
        /// <response code="200">Returns the specified BasicDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No BasicDto could be found with the provided parameters.</response>
        [HttpGet("api/basics/{id}")]
        [ProducesResponseType(typeof(BasicDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BasicDto>> GetBasicById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBasicByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;BasicDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/basics/nullable")]
        [ProducesResponseType(typeof(PagedResult<BasicDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<BasicDto>>> GetBasicsNullable(
            [FromQuery] int pageNo,
            [FromQuery] int pageSize,
            [FromQuery] string? orderBy,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBasicsNullableQuery(pageNo: pageNo, pageSize: pageSize, orderBy: orderBy), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;BasicDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/basics")]
        [ProducesResponseType(typeof(PagedResult<BasicDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<BasicDto>>> GetBasics(
            [FromQuery] int pageNo,
            [FromQuery] int pageSize,
            [FromQuery] string orderBy,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBasicsQuery(pageNo: pageNo, pageSize: pageSize, orderBy: orderBy), cancellationToken);
            return Ok(result);
        }
    }
}
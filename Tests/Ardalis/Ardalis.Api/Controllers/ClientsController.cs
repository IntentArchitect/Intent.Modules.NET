using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Api.Controllers.ResponseTypes;
using Ardalis.Application.Clients;
using Ardalis.Application.Clients.CreateClient;
using Ardalis.Application.Clients.DeleteClient;
using Ardalis.Application.Clients.GetClientById;
using Ardalis.Application.Clients.GetClients;
using Ardalis.Application.Clients.GetClientsPaginated;
using Ardalis.Application.Clients.UpdateClient;
using Ardalis.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Ardalis.Api.Controllers
{
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ISender _mediator;

        public ClientsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/client")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateClient(
            [FromBody] CreateClientCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetClientById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/client/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteClient([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteClientCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/client/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateClient(
            [FromRoute] Guid id,
            [FromBody] UpdateClientCommand command,
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
        /// <response code="200">Returns the specified ClientDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No ClientDto could be found with the provided parameters.</response>
        [HttpGet("api/client/{id}")]
        [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClientDto>> GetClientById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetClientByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;ClientDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/client/paginated")]
        [ProducesResponseType(typeof(PagedResult<ClientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<ClientDto>>> GetClientsPaginated(
            [FromQuery] int pageNo,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetClientsPaginatedQuery(pageNo: pageNo, pageSize: pageSize), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ClientDto&gt;.</response>
        [HttpGet("api/client")]
        [ProducesResponseType(typeof(List<ClientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ClientDto>>> GetClients(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetClientsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
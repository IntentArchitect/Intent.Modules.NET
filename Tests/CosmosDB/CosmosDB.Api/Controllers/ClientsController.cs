using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Api.Controllers.ResponseTypes;
using CosmosDB.Application.Clients;
using CosmosDB.Application.Clients.CreateClient;
using CosmosDB.Application.Clients.CreateClientByCtor;
using CosmosDB.Application.Clients.DeleteClient;
using CosmosDB.Application.Clients.GetClientById;
using CosmosDB.Application.Clients.GetClientByName;
using CosmosDB.Application.Clients.GetClients;
using CosmosDB.Application.Clients.GetClientsPaged;
using CosmosDB.Application.Clients.UpdateClient;
using CosmosDB.Application.Clients.UpdateClientByOp;
using CosmosDB.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CosmosDB.Api.Controllers
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
        [HttpPost("api/client/by-ctor")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateClientByCtor(
            [FromBody] CreateClientByCtorCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/clients")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateClient(
            [FromBody] CreateClientCommand command,
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
        [HttpDelete("api/clients/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteClient(
            [FromRoute] string identifier,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteClientCommand(identifier: identifier), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/clients/{identifier}/by-op")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateClientByOp(
            [FromRoute] string identifier,
            [FromBody] UpdateClientByOpCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Identifier == default)
            {
                command.Identifier = identifier;
            }

            if (identifier != command.Identifier)
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
        [HttpPut("api/clients/{identifier}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateClient(
            [FromRoute] string identifier,
            [FromBody] UpdateClientCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Identifier == default)
            {
                command.Identifier = identifier;
            }

            if (identifier != command.Identifier)
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
        [HttpGet("api/clients/by-id/{identifier}")]
        [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClientDto>> GetClientById(
            [FromRoute] string identifier,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetClientByIdQuery(identifier: identifier), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ClientDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/clients/by-name")]
        [ProducesResponseType(typeof(List<ClientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ClientDto>>> GetClientByName(
            [FromQuery] string searchText,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetClientByNameQuery(searchText: searchText), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;ClientDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/clients/paged")]
        [ProducesResponseType(typeof(PagedResult<ClientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<ClientDto>>> GetClientsPaged(
            [FromQuery] int pageNo,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetClientsPagedQuery(pageNo: pageNo, pageSize: pageSize), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ClientDto&gt;.</response>
        [HttpGet("api/clients")]
        [ProducesResponseType(typeof(List<ClientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ClientDto>>> GetClients(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetClientsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
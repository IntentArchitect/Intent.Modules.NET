using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Api.Controllers.ResponseTypes;
using CleanArchitecture.Dapr.Application.Clients;
using CleanArchitecture.Dapr.Application.Common.Eventing;
using CleanArchitecture.Dapr.Application.Common.Validation;
using CleanArchitecture.Dapr.Application.Interfaces;
using CleanArchitecture.Dapr.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CleanArchitecture.Dapr.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _appService;
        private readonly IValidationService _validationService;
        private readonly IDaprStateStoreUnitOfWork _daprStateStoreUnitOfWork;
        private readonly IEventBus _eventBus;
        public ClientsController(IClientsService appService,
            IValidationService validationService,
            IDaprStateStoreUnitOfWork daprStateStoreUnitOfWork,
            IEventBus eventBus)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _daprStateStoreUnitOfWork = daprStateStoreUnitOfWork ?? throw new ArgumentNullException(nameof(daprStateStoreUnitOfWork));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateClient(
            [FromBody] ClientCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            var result = default(string);
            result = await _appService.CreateClient(dto, cancellationToken);

            await _daprStateStoreUnitOfWork.SaveChangesAsync(cancellationToken);
            await _eventBus.FlushAllAsync(cancellationToken);
            return CreatedAtAction(nameof(FindClientById), new { id = result }, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified ClientDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No ClientDto could be found with the provided parameters.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClientDto>> FindClientById(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = default(ClientDto);
            result = await _appService.FindClientById(id, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ClientDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ClientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ClientDto>>> FindClients(CancellationToken cancellationToken = default)
        {
            var result = default(List<ClientDto>);
            result = await _appService.FindClients(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateClient(
            [FromRoute] string id,
            [FromBody] ClientUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            await _appService.UpdateClient(id, dto, cancellationToken);

            await _daprStateStoreUnitOfWork.SaveChangesAsync(cancellationToken);
            await _eventBus.FlushAllAsync(cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteClient([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            await _appService.DeleteClient(id, cancellationToken);

            await _daprStateStoreUnitOfWork.SaveChangesAsync(cancellationToken);
            await _eventBus.FlushAllAsync(cancellationToken);
            return Ok();
        }
    }
}
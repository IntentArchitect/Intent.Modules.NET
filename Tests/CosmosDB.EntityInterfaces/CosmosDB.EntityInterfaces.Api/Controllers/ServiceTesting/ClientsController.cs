using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EntityInterfaces.Application.Clients;
using CosmosDB.EntityInterfaces.Application.Common.Validation;
using CosmosDB.EntityInterfaces.Application.Interfaces.ServiceTesting;
using CosmosDB.EntityInterfaces.Domain;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Api.Controllers.ServiceTesting
{
    [ApiController]
    [Route("api/clients-non-cqrs")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _appService;

        public ClientsController(IClientsService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ClientDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ClientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ClientDto>>> GetClientsFilteredQuery(
            [FromQuery] ClientType? type,
            [FromQuery] string? name,
            CancellationToken cancellationToken = default)
        {
            var result = default(List<ClientDto>);
            result = await _appService.GetClientsFilteredQuery(type, name, cancellationToken);
            return Ok(result);
        }
    }
}
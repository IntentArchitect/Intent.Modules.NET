using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureKeyVault.Application;
using AzureKeyVault.Application.GetKeyValues;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AzureKeyVault.Api.Controllers
{
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly ISender _mediator;

        public DefaultController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified KeyValuesDTO.</response>
        [HttpGet("api/azurekeyvault-services/key-values")]
        [ProducesResponseType(typeof(KeyValuesDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<KeyValuesDTO>> GetKeyValues(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetKeyValues(), cancellationToken);
            return Ok(result);
        }
    }
}
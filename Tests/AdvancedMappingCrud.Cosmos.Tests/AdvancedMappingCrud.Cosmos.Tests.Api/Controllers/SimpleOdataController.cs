using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Api.Controllers.ResponseTypes;
using AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata;
using AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.CreateSimpleOdata;
using AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.DeleteSimpleOdata;
using AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.GetSimpleOdata;
using AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.GetSimpleOdataById;
using AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.UpdateSimpleOdata;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Validator;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Api.Controllers
{
    [ApiController]
    public class SimpleOdataController : ControllerBase
    {
        private readonly ISender _mediator;

        public SimpleOdataController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/simple-odata")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateSimpleOdata(
            [FromBody] CreateSimpleOdataCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetSimpleOdataById), new { id = result }, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/simple-odata/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteSimpleOdata(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteSimpleOdataCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/simple-odata/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateSimpleOdata(
            [FromRoute] string id,
            [FromBody] UpdateSimpleOdataCommand command,
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
        /// <response code="200">Returns the specified SimpleOdataDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No SimpleOdataDto could be found with the provided parameters.</response>
        [HttpGet("api/simple-odata/{id}")]
        [ProducesResponseType(typeof(SimpleOdataDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SimpleOdataDto>> GetSimpleOdataById(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetSimpleOdataByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;SimpleOdataDto&gt;.</response>
        [HttpGet("api/simple-odata")]
        [ProducesResponseType(typeof(List<SimpleOdataDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<SimpleOdataDto>>> GetSimpleOdata(
            ODataQueryOptions<SimpleOdataDto> oDataOptions,
            CancellationToken cancellationToken = default)
        {
            ValidateODataOptions(oDataOptions);

            var result = await _mediator.Send(new GetSimpleOdataQuery(oDataOptions.ApplyTo), cancellationToken);

            return Ok(result);
        }

        private void ValidateODataOptions<TDto>(ODataQueryOptions<TDto> options, bool enableSelect = false)
        {
            var settings = new ODataValidationSettings();

            if (!enableSelect)
            {
                settings.AllowedQueryOptions = AllowedQueryOptions.All & ~AllowedQueryOptions.Select;
            }
            options.Validate(settings);
        }
    }
}
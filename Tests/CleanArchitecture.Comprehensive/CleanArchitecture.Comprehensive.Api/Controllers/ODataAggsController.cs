using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Api.Controllers.ResponseTypes;
using CleanArchitecture.Comprehensive.Application.ODataAggs;
using CleanArchitecture.Comprehensive.Application.ODataAggs.CreateODataAgg;
using CleanArchitecture.Comprehensive.Application.ODataAggs.GetODataAggById;
using CleanArchitecture.Comprehensive.Application.ODataAggs.GetODataAggs;
using CleanArchitecture.Comprehensive.Application.ODataAggs.GetODataAggsWithSelect;
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

namespace CleanArchitecture.Comprehensive.Api.Controllers
{
    [ApiController]
    public class ODataAggsController : ControllerBase
    {
        private readonly ISender _mediator;

        public ODataAggsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/o-data-agg")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateODataAgg(
            [FromBody] CreateODataAggCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetODataAggById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified ODataAggDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No ODataAggDto could be found with the provided parameters.</response>
        [HttpGet("api/o-data-agg/{id}")]
        [ProducesResponseType(typeof(ODataAggDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ODataAggDto>> GetODataAggById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetODataAggByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ODataAggDto&gt;.</response>
        [HttpGet("api/o-data-agg")]
        [ProducesResponseType(typeof(List<ODataAggDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ODataAggDto>>> GetODataAggs(
            ODataQueryOptions<ODataAggDto> oDataOptions,
            CancellationToken cancellationToken = default)
        {
            ValidateODataOptions(oDataOptions);

            var result = await _mediator.Send(new GetODataAggsQuery(oDataOptions.ApplyTo), cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ODataAggDto&gt;.</response>
        [HttpGet("api/o-data-agg-with-select")]
        [ProducesResponseType(typeof(List<ODataAggDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetODataAggsWithSelect(
            ODataQueryOptions<ODataAggDto> oDataOptions,
            CancellationToken cancellationToken = default)
        {
            ValidateODataOptions(oDataOptions, true);

            var result = await _mediator.Send(new GetODataAggsWithSelectQuery(oDataOptions.ApplyTo), cancellationToken);

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
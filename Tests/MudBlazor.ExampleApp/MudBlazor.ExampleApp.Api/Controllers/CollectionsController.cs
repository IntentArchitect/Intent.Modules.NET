using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MudBlazor.ExampleApp.Application.Collections;
using MudBlazor.ExampleApp.Application.Collections.GetDataSingleCollection;
using MudBlazor.ExampleApp.Application.Collections.GetDataWithCollectionParams;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace MudBlazor.ExampleApp.Api.Controllers
{
    [ApiController]
    public class CollectionsController : ControllerBase
    {
        private readonly ISender _mediator;

        public CollectionsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ResponseDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/collections/data-single")]
        [ProducesResponseType(typeof(List<ResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ResponseDto>>> GetDataSingleCollection(
            [FromQuery] List<int> intCollection,
            [FromQuery] string stringValue,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDataSingleCollectionQuery(intCollection: intCollection, stringValue: stringValue), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ResponseDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/collections/data-with-params")]
        [ProducesResponseType(typeof(List<ResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ResponseDto>>> GetDataWithCollectionParams(
            [FromQuery] List<int> intCollection,
            [FromQuery] List<string> stringCollection,
            [FromQuery] int intValue,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDataWithCollectionParamsQuery(intCollection: intCollection, stringCollection: stringCollection, intValue: intValue), cancellationToken);
            return Ok(result);
        }
    }
}
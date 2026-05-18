using System.Net.Mime;
using AdvancedMapping.Repositories.Mapperly.Tests.Api.Controllers.ResponseTypes;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.Segments.CreateSegment;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Api.Controllers
{
    [ApiController]
    public class SegmentsController : ControllerBase
    {
        private readonly ISender _mediator;

        public SegmentsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/segments")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateSegment(
            [FromBody] CreateSegmentCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, new JsonResponse<Guid>(result));
        }
    }
}
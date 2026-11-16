using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Morcatko.AspNetCore.JsonMergePatch;
using SwashbuckleSettings.NET11.Api.Controllers.FileTransfer;
using SwashbuckleSettings.NET11.Api.Patching;
using SwashbuckleSettings.NET11.Application.Common.Validation;
using SwashbuckleSettings.NET11.Application.Widgets;
using SwashbuckleSettings.NET11.Application.Widgets.CreateWidget;
using SwashbuckleSettings.NET11.Application.Widgets.PatchWidget;
using SwashbuckleSettings.NET11.Application.Widgets.SearchWidgets;
using SwashbuckleSettings.NET11.Application.Widgets.UpdateWidget;
using SwashbuckleSettings.NET11.Application.Widgets.UploadWidgetImage;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace SwashbuckleSettings.NET11.Api.Controllers
{
    [ApiController]
    public class WidgetsController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly IValidatorProvider _validatorProvider;

        public WidgetsController(ISender mediator, IValidatorProvider validatorProvider)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _validatorProvider = validatorProvider ?? throw new ArgumentNullException(nameof(validatorProvider));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/widgets")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> CreateWidget(
            [FromBody] CreateWidgetCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// JSON merge patch (RFC 7396) of a widget's name.
        /// </summary>
        /// <response code="200">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No WidgetDto could be found with the provided parameters.</response>
        [HttpPatch("api/widgets/{id}")]
        [ProducesResponseType(typeof(WidgetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Consumes(JsonMergePatchDocument.ContentType)]
        public async Task<ActionResult<WidgetDto>> PatchWidget(
            [FromRoute] Guid id,
            [FromBody] JsonMergePatchDocument<PatchWidgetCommand> mergePatchDocument,
            CancellationToken cancellationToken = default)
        {
            if (mergePatchDocument == null)
            {
                return BadRequest("Merge patch document cannot be null");
            }
            var patchExecutor = new JsonMergePatchExecutor<PatchWidgetCommand>(mergePatchDocument, _validatorProvider);
            var command = new PatchWidgetCommand(id, patchExecutor);

            var result = await _mediator.Send(command, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// The route carries {id} and the command body also has Id; the OpenAPI body schema must not show id.
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/widgets/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateWidget(
            [FromRoute] Guid id,
            [FromBody] UpdateWidgetCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == Guid.Empty)
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
        /// Binary upload: stores the uploaded bytes in memory and returns a new id.
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [BinaryContent]
        [HttpPost("api/widgets/images")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> UploadWidgetImage(CancellationToken cancellationToken = default)
        {
            Stream stream;

            if (Request.ContentType != null && (Request.ContentType == "application/x-www-form-urlencoded" || Request.ContentType.StartsWith("multipart/form-data")) && Request.Form.Files.Any())
            {
                var file = Request.Form.Files[0];
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is empty");
                stream = file.OpenReadStream();
            }
            else
            {
                stream = Request.Body;
            }
            var command = new UploadWidgetImageCommand(content: stream);

            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// HTTP QUERY endpoint: the search criteria travel in the request body.
        /// </summary>
        /// <response code="200">Returns the specified List&lt;WidgetDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpQuery("api/widgets/search")]
        [ProducesResponseType(typeof(List<WidgetDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<WidgetDto>>> SearchWidgets(
            [FromBody] SearchWidgetsQuery query,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
using System.Net.Http.Headers;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Morcatko.AspNetCore.JsonMergePatch;
using Scalar.NET11.Bearer.Api.Controllers.FileTransfer;
using Scalar.NET11.Bearer.Api.Patching;
using Scalar.NET11.Bearer.Application;
using Scalar.NET11.Bearer.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Scalar.NET11.Bearer.Api.Controllers
{
    [ApiController]
    public class WidgetController : ControllerBase
    {
        private readonly IWidgetService _appService;

        public WidgetController(IWidgetService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
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
            [FromBody] WidgetDto dto,
            CancellationToken cancellationToken = default)
        {
            var result = Guid.Empty;
            result = await _appService.CreateWidget(dto, cancellationToken);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// The route carries {id} and WidgetDto also has Id; the OpenAPI body schema must not show id.
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
            [FromBody] WidgetDto dto,
            CancellationToken cancellationToken = default)
        {
            await _appService.UpdateWidget(id, dto, cancellationToken);
            return NoContent();
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
            [FromBody] WidgetSearchCriteriaDto criteria,
            CancellationToken cancellationToken = default)
        {
            var result = default(List<WidgetDto>);
            result = await _appService.SearchWidgets(criteria, cancellationToken);
            return Ok(result);
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
        public async Task<ActionResult<Guid>> UploadWidgetImage(
            [FromHeader(Name = "Content-Type")] string? contentType,
            [FromHeader(Name = "Content-Length")] long? contentLength,
            CancellationToken cancellationToken = default)
        {
            Stream stream;
            string? filename = null;
            if (Request.Headers.TryGetValue("Content-Disposition", out var headerValues))
            {
                string? header = headerValues;
                if (header != null)
                {
                    var contentDisposition = ContentDispositionHeaderValue.Parse(header);
                    filename = contentDisposition?.FileName;
                }
            }

            if (Request.ContentType != null && (Request.ContentType == "application/x-www-form-urlencoded" || Request.ContentType.StartsWith("multipart/form-data")) && Request.Form.Files.Any())
            {
                var file = Request.Form.Files[0];
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is empty");
                stream = file.OpenReadStream();
                filename ??= file.Name;
            }
            else
            {
                stream = Request.Body;
            }
            var result = Guid.Empty;
            result = await _appService.UploadWidgetImage(stream, filename, contentType, contentLength, cancellationToken);
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
            [FromBody] JsonMergePatchDocument<PatchWidgetDto> mergePatchDocument,
            CancellationToken cancellationToken = default)
        {
            if (mergePatchDocument == null)
            {
                return BadRequest("Merge patch document cannot be null");
            }

            var patchExecutor = new JsonMergePatchExecutor<PatchWidgetDto>(mergePatchDocument);

            var dto = new PatchWidgetDto
            {
                PatchExecutor = patchExecutor
            };

            var result = default(WidgetDto);
            result = await _appService.PatchWidget(id, dto, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
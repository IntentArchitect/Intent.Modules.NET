using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Api.Controllers.FileTransfer;
using AdvancedMappingCrud.Repositories.Tests.Api.Controllers.ResponseTypes;
using AdvancedMappingCrud.Repositories.Tests.Application.Common;
using AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.DownloadFile;
using AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.RestrictedUpload;
using AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.SimpleDownload;
using AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.SimpleUpload;
using AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.UploadFile;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Api.Controllers
{
    [ApiController]
    public class FileUploadsController : ControllerBase
    {
        private readonly ISender _mediator;

        public FileUploadsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [BinaryContent]
        [HttpPost("api/file-uploads/restricted-upload")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RestrictedUpload(
            [FromHeader(Name = "Content-Type")] string? contentType,
            [FromHeader(Name = "Content-Length")] long? contentLength,
            CancellationToken cancellationToken = default)
        {
            if (Request.ContentLength != null && Request.ContentLength > 1000000)
            {
                return BadRequest(new { error = "File to large." });
            }
            var mimeTypeFilter = new HashSet<string>(@"text/plain,image/png".Split(','));

            if (Request.ContentType == null || !mimeTypeFilter.Contains(Request.ContentType.ToLower()))
            {
                return BadRequest(new { error = $"Invalid file type. {Request.ContentType} not allowed." });
            }
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
            var command = new RestrictedUploadCommand(content: stream, filename: filename, contentType: contentType, contentLength: contentLength);

            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [BinaryContent]
        [HttpPost("api/file-uploads/simple-upload")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> SimpleUpload(CancellationToken cancellationToken = default)
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
            var command = new SimpleUploadCommand(content: stream);

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(DownloadFile), new { id = result }, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [BinaryContent]
        [HttpPost("api/file-uploads/upload-file")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> UploadFile(
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
            var command = new UploadFileCommand(content: stream, filename: filename, contentType: contentType, contentLength: contentLength);

            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified byte[].</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No byte[] could be found with the provided parameters.</response>
        [HttpGet("api/file-upload/{id}")]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<byte[]>> DownloadFile(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new DownloadFileQuery(id: id), cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return File(result.Content, result.ContentType ?? "application/octet-stream", result.Filename);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified byte[].</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No byte[] could be found with the provided parameters.</response>
        [HttpGet("api/file-uploads/simple-download")]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<byte[]>> SimpleDownload(
            [FromQuery] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new SimpleDownloadQuery(id: id), cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return File(result.Content, "application/octet-stream");
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.DependencyInjection;
using OutputCachingRedis.Tests.Api.Controllers.FileTransfer;
using OutputCachingRedis.Tests.Api.Controllers.ResponseTypes;
using OutputCachingRedis.Tests.Application.Common;
using OutputCachingRedis.Tests.Application.Files.CreateFiles;
using OutputCachingRedis.Tests.Application.Files.GetFilesById;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace OutputCachingRedis.Tests.Api.Controllers
{
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ISender _mediator;

        public FilesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [BinaryContent]
        [HttpPost("api/file")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateFiles(
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
            var command = new CreateFilesCommand(content: stream, filename: filename, contentType: contentType, contentLength: contentLength);

            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified byte[].</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No byte[] could be found with the provided parameters.</response>
        [HttpGet("api/file/{id}")]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [OutputCache(PolicyName = "Long Term", Tags = ["file"])]
        public async Task<ActionResult<byte[]>> GetFilesById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetFilesByIdQuery(id: id), cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return File(result.Content, result.ContentType ?? "application/octet-stream", result.Filename);
        }
    }
}
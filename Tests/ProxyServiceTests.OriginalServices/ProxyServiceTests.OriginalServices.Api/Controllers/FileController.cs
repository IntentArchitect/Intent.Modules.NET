using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ProxyServiceTests.OriginalServices.Api.Controllers.FileTransfer;
using ProxyServiceTests.OriginalServices.Application.File.FileUpload;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Api.Controllers
{
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly ISender _mediator;

        public FileController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [BinaryContent]
        [HttpPost("api/file/upload")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> FileUpload(
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
            var command = new FileUploadCommand(content: stream, filename: filename, contentType: contentType, contentLength: contentLength);

            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }
    }
}
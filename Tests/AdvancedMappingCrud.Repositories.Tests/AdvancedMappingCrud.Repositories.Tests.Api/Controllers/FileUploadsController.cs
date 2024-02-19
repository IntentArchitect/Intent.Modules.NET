using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Api.Controllers.FileTransfer;
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
        [HttpPost("api/file-uploads/upload-file")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> UploadFile(
            [FromQuery] string? filename,
            [FromQuery] string? contentType,
            CancellationToken cancellationToken = default)
        {
            Stream stream;

            if ((Request.ContentType != null &&
                (Request.ContentType == "application/x-www-form-urlencoded" || Request.ContentType.StartsWith("multipart/form-data")) &&
                Request.Form.Files.Any()))
            {
                var file = Request.Form.Files[0];

                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is empty");
                stream = file.OpenReadStream();

                if (filename == null)
                {
                    filename = file.Name;
                }

                if (contentType == null)
                {
                    contentType = file.ContentType;
                }
            }
            else
            {
                stream = Request.Body;
            }
            var command = new UploadFile(content: stream, contentType: Request.ContentType ?? "application/octet-stream", filename: filename);

            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, result);
        }
    }
}
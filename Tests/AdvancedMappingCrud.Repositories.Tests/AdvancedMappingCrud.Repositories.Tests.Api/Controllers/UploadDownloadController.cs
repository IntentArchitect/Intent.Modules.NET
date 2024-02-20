using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AdvancedMappingCrud.Repositories.Tests.Api.Controllers.FileTransfer;
using AdvancedMappingCrud.Repositories.Tests.Application.Common;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Eventing;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Validation;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Api.Controllers
{
    [ApiController]
    [Route("api/upload-download")]
    public class UploadDownloadController : ControllerBase
    {
        private readonly IUploadDownloadService _appService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;

        public UploadDownloadController(IUploadDownloadService appService, IUnitOfWork unitOfWork, IEventBus eventBus)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [BinaryContent]
        [HttpPost("upload")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Upload(
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
            var result = default(Guid);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.Upload(stream, filename, contentType, contentLength, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(cancellationToken);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified byte[].</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No byte[] could be found with the provided parameters.</response>
        [HttpGet("download")]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<byte[]>> Download(
            [FromQuery] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = default(FileDownloadDto);
            result = await _appService.Download(id, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return File(result.Content, result.ContentType ?? "application/octet-stream", result.Filename);
        }
    }
}
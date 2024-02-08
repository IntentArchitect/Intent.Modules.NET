using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.MongoDb.Application;
using Entities.PrivateSetters.MongoDb.Application.Interfaces;
using Entities.PrivateSetters.MongoDb.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _appService;
        private readonly IMongoDbUnitOfWork _mongoDbUnitOfWork;

        public InvoiceController(IInvoiceService appService, IMongoDbUnitOfWork mongoDbUnitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _mongoDbUnitOfWork = mongoDbUnitOfWork ?? throw new ArgumentNullException(nameof(mongoDbUnitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Create(
            [FromBody] CreateInvoiceDto dto,
            CancellationToken cancellationToken = default)
        {
            await _appService.Create(dto, cancellationToken);

            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;InvoiceDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<InvoiceDto>>> GetAll(CancellationToken cancellationToken = default)
        {
            var result = default(List<InvoiceDto>);
            result = await _appService.GetAll(cancellationToken);
            return Ok(result);
        }
    }
}
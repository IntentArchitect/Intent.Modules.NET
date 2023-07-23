using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDb.TestApplication.Application;
using MongoDb.TestApplication.Application.DerivedOfTS;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace MongoDb.TestApplication.Api.Controllers
{
    [ApiController]
    [Route("api/derived-of-t-s")]
    public class DerivedOfTSController : ControllerBase
    {
        private readonly IDerivedOfTSService _appService;
        private readonly IValidationService _validationService;
        private readonly IMongoDbUnitOfWork _mongoDbUnitOfWork;

        public DerivedOfTSController(IDerivedOfTSService appService,
            IValidationService validationService,
            IMongoDbUnitOfWork mongoDbUnitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _mongoDbUnitOfWork = mongoDbUnitOfWork ?? throw new ArgumentNullException(nameof(mongoDbUnitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CreateDerivedOfT(
            [FromBody] DerivedOfTCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            var result = default(string);
            result = await _appService.CreateDerivedOfT(dto, cancellationToken);
            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified DerivedOfTDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an DerivedOfTDto with the parameters provided.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DerivedOfTDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DerivedOfTDto>> FindDerivedOfTById(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = default(DerivedOfTDto);
            result = await _appService.FindDerivedOfTById(id, cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;DerivedOfTDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<DerivedOfTDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<DerivedOfTDto>>> FindDerivedOfTS(CancellationToken cancellationToken = default)
        {
            var result = default(List<DerivedOfTDto>);
            result = await _appService.FindDerivedOfTS(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateDerivedOfT(
            [FromRoute] string id,
            [FromBody] DerivedOfTUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            await _appService.UpdateDerivedOfT(id, dto, cancellationToken);
            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteDerivedOfT(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            await _appService.DeleteDerivedOfT(id, cancellationToken);
            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return Ok();
        }
    }
}
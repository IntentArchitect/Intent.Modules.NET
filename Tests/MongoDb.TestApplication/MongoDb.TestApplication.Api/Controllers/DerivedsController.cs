using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDb.TestApplication.Application;
using MongoDb.TestApplication.Application.Deriveds;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace MongoDb.TestApplication.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DerivedsController : ControllerBase
    {
        private readonly IDerivedsService _appService;
        private readonly IValidationService _validationService;
        private readonly IMongoDbUnitOfWork _mongoDbUnitOfWork;

        public DerivedsController(IDerivedsService appService,
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
        public async Task<ActionResult<string>> CreateDerived(
            [FromBody] DerivedCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            var result = default(string);
            result = await _appService.CreateDerived(dto, cancellationToken);

            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return CreatedAtAction(nameof(FindDerivedById), new { id = result }, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified DerivedDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No DerivedDto could be found with the provided parameters.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DerivedDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DerivedDto>> FindDerivedById(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = default(DerivedDto);
            result = await _appService.FindDerivedById(id, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;DerivedDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<DerivedDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<DerivedDto>>> FindDeriveds(CancellationToken cancellationToken = default)
        {
            var result = default(List<DerivedDto>);
            result = await _appService.FindDeriveds(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateDerived(
            [FromRoute] string id,
            [FromBody] DerivedUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            await _appService.UpdateDerived(id, dto, cancellationToken);

            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteDerived([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            await _appService.DeleteDerived(id, cancellationToken);

            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return Ok();
        }
    }
}
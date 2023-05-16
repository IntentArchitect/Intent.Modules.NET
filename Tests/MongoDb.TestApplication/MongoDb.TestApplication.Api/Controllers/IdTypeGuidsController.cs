using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDb.TestApplication.Application.IdTypeGuids;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace MongoDb.TestApplication.Api.Controllers
{
    [ApiController]
    [Route("api/id-type-guids")]
    public class IdTypeGuidsController : ControllerBase
    {
        private readonly IIdTypeGuidsService _appService;
        private readonly IMongoDbUnitOfWork _mongoDbUnitOfWork;

        public IdTypeGuidsController(IIdTypeGuidsService appService, IMongoDbUnitOfWork mongoDbUnitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _mongoDbUnitOfWork = mongoDbUnitOfWork ?? throw new ArgumentNullException(nameof(mongoDbUnitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> CreateIdTypeGuid(
            [FromBody] IdTypeGuidCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var result = default(Guid);
            result = await _appService.CreateIdTypeGuid(dto);
            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified IdTypeGuidDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an IdTypeGuidDto with the parameters provided.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IdTypeGuidDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IdTypeGuidDto>> FindIdTypeGuidById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = default(IdTypeGuidDto);
            result = await _appService.FindIdTypeGuidById(id);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;IdTypeGuidDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<IdTypeGuidDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<IdTypeGuidDto>>> FindIdTypeGuids(CancellationToken cancellationToken = default)
        {
            var result = default(List<IdTypeGuidDto>);
            result = await _appService.FindIdTypeGuids();
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateIdTypeGuid(
            [FromRoute] Guid id,
            [FromBody] IdTypeGuidUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _appService.UpdateIdTypeGuid(id, dto);
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteIdTypeGuid(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _appService.DeleteIdTypeGuid(id);
            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return Ok();
        }
    }
}
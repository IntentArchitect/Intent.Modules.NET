using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Application.MapperRoots;
using MongoDb.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace MongoDb.TestApplication.Api.Controllers
{
    [ApiController]
    [Route("api/mapper-roots")]
    public class MapperRootsController : ControllerBase
    {
        private readonly IMapperRootsService _appService;
        private readonly IMongoDbUnitOfWork _mongoDbUnitOfWork;

        public MapperRootsController(IMapperRootsService appService, IMongoDbUnitOfWork mongoDbUnitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _mongoDbUnitOfWork = mongoDbUnitOfWork ?? throw new ArgumentNullException(nameof(mongoDbUnitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CreateMapperRoot(
            [FromBody] MapperRootCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var result = default(string);
            result = await _appService.CreateMapperRoot(dto, cancellationToken);
            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified MapperRootDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an MapperRootDto with the parameters provided.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MapperRootDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MapperRootDto>> FindMapperRootById(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = default(MapperRootDto);
            result = await _appService.FindMapperRootById(id, cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;MapperRootDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<MapperRootDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<MapperRootDto>>> FindMapperRoots(CancellationToken cancellationToken = default)
        {
            var result = default(List<MapperRootDto>);
            result = await _appService.FindMapperRoots(cancellationToken);
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
        public async Task<ActionResult> UpdateMapperRoot(
            [FromRoute] string id,
            [FromBody] MapperRootUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _appService.UpdateMapperRoot(id, dto, cancellationToken);
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
        public async Task<ActionResult> DeleteMapperRoot(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            await _appService.DeleteMapperRoot(id, cancellationToken);
            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return Ok();
        }
    }
}
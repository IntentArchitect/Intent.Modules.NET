using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OpenApiImporterTest.Application.Pets;
using OpenApiImporterTest.Application.Pets.CreatePet;
using OpenApiImporterTest.Application.Pets.DeletePet;
using OpenApiImporterTest.Application.Pets.GetPet;
using OpenApiImporterTest.Application.Pets.GetPetFindByStatus;
using OpenApiImporterTest.Application.Pets.GetPetFindByTags;
using OpenApiImporterTest.Application.Pets.UpdatePet;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace OpenApiImporterTest.Api.Controllers
{
    [ApiController]
    public class PetsController : ControllerBase
    {
        private readonly ISender _mediator;

        public PetsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("/pet/")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Pet), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Pet>> CreatePet(
            [FromBody] CreatePetCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("/pet/{petId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeletePet(
            [FromHeader(Name = "api_key")] string api_key,
            [FromRoute] int petId,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeletePetCommand(api_key: api_key, petId: petId), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("/pet/")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Pet), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Pet>> UpdatePet(
            [FromBody] UpdatePetCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;Pet&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("/pet/findByStatus")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(List<Pet>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Pet>>> GetPetFindByStatus(
            [FromQuery] string status,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPetFindByStatusQuery(status: status), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;Pet&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("/pet/findByTags")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(List<Pet>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Pet>>> GetPetFindByTags(
            [FromQuery] List<string> tags,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPetFindByTagsQuery(tags: tags), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified Pet.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No Pet could be found with the provided parameters.</response>
        [HttpGet("/pet/{petId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Pet), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Pet>> GetPet([FromRoute] int petId, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPetQuery(petId: petId), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
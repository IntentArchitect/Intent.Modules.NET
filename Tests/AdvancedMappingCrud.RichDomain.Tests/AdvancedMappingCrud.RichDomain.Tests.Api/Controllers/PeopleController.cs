using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Api.Controllers.ResponseTypes;
using AdvancedMappingCrud.RichDomain.Tests.Application.People;
using AdvancedMappingCrud.RichDomain.Tests.Application.People.CreatePerson;
using AdvancedMappingCrud.RichDomain.Tests.Application.People.DeletePerson;
using AdvancedMappingCrud.RichDomain.Tests.Application.People.GetPeople;
using AdvancedMappingCrud.RichDomain.Tests.Application.People.GetPersonById;
using AdvancedMappingCrud.RichDomain.Tests.Application.People.UpdatePerson;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Api.Controllers
{
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ISender _mediator;

        public PeopleController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/person")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreatePerson(
            [FromBody] CreatePersonCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetPersonById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/person/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeletePerson([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeletePersonCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/person/{id}/")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdatePerson(
            [FromRoute] Guid id,
            [FromBody] UpdatePersonCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == default)
            {
                command.Id = id;
            }

            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;PersonDto&gt;.</response>
        [HttpGet("api/person")]
        [ProducesResponseType(typeof(List<PersonDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PersonDto>>> GetPeople(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPeopleQuery(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PersonDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No PersonDto could be found with the provided parameters.</response>
        [HttpGet("api/person/{id}")]
        [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PersonDto>> GetPersonById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPersonByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
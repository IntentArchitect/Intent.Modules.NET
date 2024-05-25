using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Api.Controllers.ResponseTypes;
using AdvancedMappingCrud.RichDomain.Tests.Application.Users;
using AdvancedMappingCrud.RichDomain.Tests.Application.Users.AddCollectionsUser;
using AdvancedMappingCrud.RichDomain.Tests.Application.Users.CreateUser;
using AdvancedMappingCrud.RichDomain.Tests.Application.Users.DeleteUser;
using AdvancedMappingCrud.RichDomain.Tests.Application.Users.GetUserById;
using AdvancedMappingCrud.RichDomain.Tests.Application.Users.GetUsers;
using AdvancedMappingCrud.RichDomain.Tests.Application.Users.TestDCUser;
using AdvancedMappingCrud.RichDomain.Tests.Application.Users.TestEntityUser;
using AdvancedMappingCrud.RichDomain.Tests.Application.Users.TestVOUser;
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
    public class UsersController : ControllerBase
    {
        private readonly ISender _mediator;

        public UsersController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("api/user/{id}/collections")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddCollectionsUser(
            [FromRoute] Guid id,
            [FromBody] AddCollectionsUserCommand command,
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
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/user")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateUser(
            [FromBody] CreateUserCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetUserById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUser([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteUserCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("api/user/{id}/test-d-c")]
        [ProducesResponseType(typeof(TestAddressDCDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TestAddressDCDto?>> TestDCUser(
            [FromRoute] Guid id,
            [FromBody] TestDCUserCommand command,
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

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No TestCompanyDto could be found with the provided parameters.</response>
        [HttpPut("api/user/{id}/test-entity")]
        [ProducesResponseType(typeof(TestCompanyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TestCompanyDto>> TestEntityUser(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new TestEntityUserCommand(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No TestContactDetailsVODto could be found with the provided parameters.</response>
        [HttpPut("api/user/{id}/test-v-o")]
        [ProducesResponseType(typeof(TestContactDetailsVODto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TestContactDetailsVODto>> TestVOUser(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new TestVOUserCommand(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified UserDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No UserDto could be found with the provided parameters.</response>
        [HttpGet("api/user/{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> GetUserById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;UserDto&gt;.</response>
        [HttpGet("api/user")]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserDto>>> GetUsers(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetUsersQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
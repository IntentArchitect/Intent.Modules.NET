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
using OpenApiImporterTest.Api.Controllers.ResponseTypes;
using OpenApiImporterTest.Application.Users;
using OpenApiImporterTest.Application.Users.CreateUser;
using OpenApiImporterTest.Application.Users.DeleteUser;
using OpenApiImporterTest.Application.Users.GetLogin;
using OpenApiImporterTest.Application.Users.GetLogout;
using OpenApiImporterTest.Application.Users.GetUser;
using OpenApiImporterTest.Application.Users.UpdateUser;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace OpenApiImporterTest.Api.Controllers
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
        [HttpPost("/user/")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateUser(
            [FromBody] CreateUserCommand command,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpDelete("/user/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUser(
            [FromRoute] string username,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteUserCommand(username: username), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("/user/{username}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUser(
            [FromRoute] string username,
            [FromBody] UpdateUserCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Username == default)
            {
                command.Username = username;
            }

            if (username != command.Username)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified string.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpGet("/user/login")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> GetLogin(
            [FromQuery] string username,
            [FromQuery] string password,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetLoginQuery(username: username, password: password), cancellationToken);
            return result == null ? NotFound() : Ok(new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified int.</response>
        [HttpGet("/user/logout")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<int>>> GetLogout(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetLogoutQuery(), cancellationToken);
            return Ok(new JsonResponse<int>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified User.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No User could be found with the provided parameters.</response>
        [HttpGet("/user/{username}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> GetUser(
            [FromRoute] string username,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetUserQuery(username: username), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
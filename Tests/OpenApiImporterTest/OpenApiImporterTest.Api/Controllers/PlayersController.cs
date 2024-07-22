using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OpenApiImporterTest.Application.Players;
using OpenApiImporterTest.Application.Players.CreateBetsMax;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace OpenApiImporterTest.Api.Controllers
{
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly ISender _mediator;

        public PlayersController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="401">Unauthorized request.</response>
        /// <response code="403">Forbidden request.</response>
        /// <response code="404">No MaxBetResultViewModel could be found with the provided parameters.</response>
        [HttpPost("/api/players/{playerId}/Bets/max")]
        [Authorize]
        [ProducesResponseType(typeof(MaxBetResultViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MaxBetResultViewModel>> CreateBetsMax(
            [FromRoute] string playerId,
            [FromBody] CreateBetsMaxCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.PlayerId == default)
            {
                command.PlayerId = playerId;
            }

            if (playerId != command.PlayerId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return result == null ? NotFound() : Created(string.Empty, result);
        }
    }
}
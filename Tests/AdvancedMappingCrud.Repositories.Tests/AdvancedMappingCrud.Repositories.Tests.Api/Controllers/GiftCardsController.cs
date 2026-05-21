using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Api.Controllers.ResponseTypes;
using AdvancedMappingCrud.Repositories.Tests.Application.GiftCards;
using AdvancedMappingCrud.Repositories.Tests.Application.GiftCards.CreateGiftCard;
using AdvancedMappingCrud.Repositories.Tests.Application.GiftCards.DeleteGiftCard;
using AdvancedMappingCrud.Repositories.Tests.Application.GiftCards.GetGiftCardById;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Api.Controllers
{
    [ApiController]
    public class GiftCardsController : ControllerBase
    {
        private readonly ISender _mediator;

        public GiftCardsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/gift-cards/{cardCode}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateGiftCard(
            [FromRoute] string cardCode,
            [FromBody] CreateGiftCardCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.CardCode == default)
            {
                command.CardCode = cardCode;
            }

            if (cardCode != command.CardCode)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpDelete("api/gift-cards/{cardCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteGiftCard(
            [FromRoute] string cardCode,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteGiftCardCommand(cardCode: cardCode), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified GiftCardDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No GiftCardDto could be found with the provided parameters.</response>
        [HttpGet("api/gift-cards/{cardCode}")]
        [ProducesResponseType(typeof(GiftCardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GiftCardDto>> GetGiftCardById(
            [FromRoute] string cardCode,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetGiftCardByIdQuery(cardCode: cardCode), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
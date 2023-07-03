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
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Api.Controllers.ResponseTypes;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.CreateBasket;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.CreateBasketBasketItem;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.DeleteBasket;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.DeleteBasketBasketItem;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.GetBasketBasketItemById;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.GetBasketBasketItems;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.GetBasketById;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.GetBaskets;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.UpdateBasket;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.UpdateBasketBasketItem;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Api.Controllers
{
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly ISender _mediator;

        public BasketsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/baskets/{basketId}/basket-items")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> CreateBasketBasketItem(
            [FromRoute] Guid basketId,
            [FromBody] CreateBasketBasketItemCommand command,
            CancellationToken cancellationToken = default)
        {
            if (basketId != command.BasketId)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetBasketById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/baskets")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> CreateBasket(
            [FromBody] CreateBasketCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetBasketById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpDelete("api/baskets/{basketId}/basket-items/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteBasketBasketItem(
            [FromRoute] Guid basketId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteBasketBasketItemCommand(basketId: basketId, id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpDelete("api/baskets/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteBasket([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteBasketCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("api/baskets/{basketId}/basket-items/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateBasketBasketItem(
            [FromRoute] Guid basketId,
            [FromRoute] Guid id,
            [FromBody] UpdateBasketBasketItemCommand command,
            CancellationToken cancellationToken = default)
        {
            if (basketId != command.BasketId)
            {
                return BadRequest();
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
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("api/baskets/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateBasket(
            [FromRoute] Guid id,
            [FromBody] UpdateBasketCommand command,
            CancellationToken cancellationToken = default)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified BasketBasketItemDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an BasketBasketItemDto with the parameters provided.</response>
        [HttpGet("api/baskets/{basketId}/basket-items/{id}")]
        [ProducesResponseType(typeof(BasketBasketItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BasketBasketItemDto>> GetBasketBasketItemById(
            [FromRoute] Guid basketId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBasketBasketItemByIdQuery(basketId: basketId, id: id), cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;BasketBasketItemDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/baskets/{basketId}/basket-items")]
        [ProducesResponseType(typeof(List<BasketBasketItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BasketBasketItemDto>>> GetBasketBasketItems(
            [FromRoute] Guid basketId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBasketBasketItemsQuery(basketId: basketId), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified BasketDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an BasketDto with the parameters provided.</response>
        [HttpGet("api/baskets/{id}")]
        [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BasketDto>> GetBasketById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBasketByIdQuery(id: id), cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;BasketDto&gt;.</response>
        [HttpGet("api/baskets")]
        [ProducesResponseType(typeof(List<BasketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BasketDto>>> GetBaskets(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBasketsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
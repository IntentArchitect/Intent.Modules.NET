using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Wolverine.CQRS.TestApplication.Application.Items;
using Wolverine.CQRS.TestApplication.Application.Items.CreateItem;
using Wolverine.CQRS.TestApplication.Application.Items.GetItemById;
using Wolverine.CQRS.TestApplication.Application.Items.GetItems;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Api.Controllers
{
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IMessageBus _messageBus;

        public ItemsController(IMessageBus messageBus)
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/items")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> CreateItem(
            [FromBody] CreateItemCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _messageBus.InvokeAsync<Guid>(command, cancellationToken);
            return CreatedAtAction(nameof(GetItemById), new { id = result }, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified ItemDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No ItemDto could be found with the provided parameters.</response>
        [HttpGet("api/items/{id}")]
        [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ItemDto>> GetItemById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _messageBus.InvokeAsync<ItemDto>(new GetItemByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ItemDto&gt;.</response>
        [HttpGet("api/items")]
        [ProducesResponseType(typeof(List<ItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ItemDto>>> GetItems(CancellationToken cancellationToken = default)
        {
            var result = await _messageBus.InvokeAsync<List<ItemDto>>(new GetItemsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}

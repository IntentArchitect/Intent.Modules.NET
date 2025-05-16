using System.Net.Mime;
using Intent.Modules.NET.Tests.Application.Core.Common.Eventing;
using Intent.Modules.NET.Tests.Module1.Api.Controllers.ResponseTypes;
using Intent.Modules.NET.Tests.Module1.Application.Common.Validation;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Interfaces;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Orders;
using Intent.Modules.NET.Tests.Module1.Application.Orders;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _appService;
        private readonly IValidationService _validationService;
        private readonly IEventBus _eventBus;

        public OrdersController(IOrdersService appService, IValidationService validationService, IEventBus eventBus)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateOrder(
            [FromBody] OrderCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            var result = Guid.Empty;
            result = await _appService.CreateOrder(dto, cancellationToken);
            await _eventBus.FlushAllAsync(cancellationToken);
            return CreatedAtAction(nameof(FindOrderById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateOrder(
            [FromRoute] Guid id,
            [FromBody] OrderUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            await _appService.UpdateOrder(id, dto, cancellationToken);
            await _eventBus.FlushAllAsync(cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified OrderDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No OrderDto could be found with the provided parameters.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDto>> FindOrderById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = default(OrderDto);
            result = await _appService.FindOrderById(id, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;OrderDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<OrderDto>>> FindOrders(CancellationToken cancellationToken = default)
        {
            var result = default(List<OrderDto>);
            result = await _appService.FindOrders(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteOrder([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            await _appService.DeleteOrder(id, cancellationToken);
            await _eventBus.FlushAllAsync(cancellationToken);
            return Ok();
        }
    }
}
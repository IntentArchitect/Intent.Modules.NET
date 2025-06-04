using System.Net.Mime;
using AspNetCoreCleanArchitecture.Sample.Api.Controllers.ResponseTypes;
using AspNetCoreCleanArchitecture.Sample.Application.Buyers;
using AspNetCoreCleanArchitecture.Sample.Application.Buyers.ActivateBuyer;
using AspNetCoreCleanArchitecture.Sample.Application.Buyers.CreateBuyer;
using AspNetCoreCleanArchitecture.Sample.Application.Buyers.DeleteBuyer;
using AspNetCoreCleanArchitecture.Sample.Application.Buyers.GetBuyerById;
using AspNetCoreCleanArchitecture.Sample.Application.Buyers.GetBuyers;
using AspNetCoreCleanArchitecture.Sample.Application.Buyers.GetBuyerStatistics;
using AspNetCoreCleanArchitecture.Sample.Application.Buyers.UpdateBuyer;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Api.Controllers
{
    [ApiController]
    public class BuyersController : ControllerBase
    {
        private readonly ISender _mediator;

        public BuyersController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("api/buyers/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActivateBuyer(
            [FromBody] ActivateBuyerCommand command,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/buyers")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateBuyer(
            [FromBody] CreateBuyerCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetBuyerById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/buyers/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteBuyer([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteBuyerCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/buyers/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateBuyer(
            [FromRoute] Guid id,
            [FromBody] UpdateBuyerCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == Guid.Empty)
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
        /// <response code="200">Returns the specified BuyerDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No BuyerDto could be found with the provided parameters.</response>
        [HttpGet("api/buyers/{id}")]
        [ProducesResponseType(typeof(BuyerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BuyerDto>> GetBuyerById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBuyerByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;BuyerDto&gt;.</response>
        [HttpGet("api/buyers")]
        [ProducesResponseType(typeof(List<BuyerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BuyerDto>>> GetBuyers(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBuyersQuery(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified BuyerStatisticsDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No BuyerStatisticsDto could be found with the provided parameters.</response>
        [HttpGet("api/buyers/statistics")]
        [ProducesResponseType(typeof(BuyerStatisticsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BuyerStatisticsDto>> GetBuyerStatistics(
            [FromQuery] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBuyerStatisticsQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
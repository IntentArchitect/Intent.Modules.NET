using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.CreatePurchase;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.DeletePurchase;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.GetPurchaseById;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.GetPurchases;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.UpdatePurchase;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Api.Controllers
{
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly ISender _mediator;

        public PurchasesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/purchases")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(PurchaseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PurchaseDto>> CreatePurchase(
            [FromBody] CreatePurchaseCommand command,
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
        [HttpDelete("api/purchases/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeletePurchase([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeletePurchaseCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/purchases/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdatePurchase(
            [FromRoute] Guid id,
            [FromBody] UpdatePurchaseCommand command,
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
        /// <response code="200">Returns the specified PurchaseDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No PurchaseDto could be found with the provided parameters.</response>
        [HttpGet("api/purchases/{id}")]
        [ProducesResponseType(typeof(PurchaseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PurchaseDto>> GetPurchaseById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPurchaseByIdQuery(id: id), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;PurchaseDto&gt;.</response>
        [HttpGet("api/purchases")]
        [ProducesResponseType(typeof(List<PurchaseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PurchaseDto>>> GetPurchases(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPurchasesQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
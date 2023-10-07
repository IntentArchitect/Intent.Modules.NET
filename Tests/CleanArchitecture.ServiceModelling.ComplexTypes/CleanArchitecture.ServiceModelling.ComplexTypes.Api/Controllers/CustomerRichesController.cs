using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.ServiceModelling.ComplexTypes.Api.Controllers.ResponseTypes;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.ChangeAddress;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.CheckResult;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.CreateCustomerRich;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.DeleteCustomerRich;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.GetCustomerRichById;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.GetCustomerRiches;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.UpdateCustomerRich;
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
    public class CustomerRichesController : ControllerBase
    {
        private readonly ISender _mediator;

        public CustomerRichesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/customer-riches/{id}/change-address")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangeAddress(
            [FromRoute] Guid id,
            [FromBody] ChangeAddressCommand command,
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
        /// <response code="200">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No CheckAddressDCDto could be found with the provided parameters.</response>
        [HttpPut("api/customer-riches/{id}/check-result")]
        [ProducesResponseType(typeof(CheckAddressDCDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CheckAddressDCDto>> CheckResult(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CheckResultCommand(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/customer-riches")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateCustomerRich(
            [FromBody] CreateCustomerRichCommand command,
            CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetCustomerRichById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/customer-riches/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCustomerRich(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteCustomerRichCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/customer-riches/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCustomerRich(
            [FromRoute] Guid id,
            [FromBody] UpdateCustomerRichCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == default)
            {
                command.SetId(id);
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
        /// <response code="200">Returns the specified CustomerRichDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No CustomerRichDto could be found with the provided parameters.</response>
        [HttpGet("api/customer-riches/{id}")]
        [ProducesResponseType(typeof(CustomerRichDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerRichDto>> GetCustomerRichById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomerRichByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;CustomerRichDto&gt;.</response>
        [HttpGet("api/customer-riches")]
        [ProducesResponseType(typeof(List<CustomerRichDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CustomerRichDto>>> GetCustomerRiches(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomerRichesQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
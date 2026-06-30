using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ObjectMappingTest.Application.Customers;
using ObjectMappingTest.Application.Customers.GetCustomerById;
using ObjectMappingTest.Application.Customers.GetCustomerDetail;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace ObjectMappingTest.Api.Controllers
{
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ISender _mediator;

        public CustomersController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified CustomerDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No CustomerDto could be found with the provided parameters.</response>
        [HttpGet("api/customers/{id}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerDto>> GetCustomerById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomerById(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified CustomerDetailDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No CustomerDetailDto could be found with the provided parameters.</response>
        [HttpGet("api/customers/{id}/detail")]
        [ProducesResponseType(typeof(CustomerDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerDetailDto>> GetCustomerDetail(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomerDetail(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
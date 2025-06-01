using Intent.Modules.NET.Tests.Module2.Application.Contracts.MyCustomers;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.MyCustomers.GetMyCustomerById;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.MyCustomers.GetMyCustomers;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Api.Controllers
{
    [ApiController]
    public class MyCustomersController : ControllerBase
    {
        private readonly ISender _mediator;

        public MyCustomersController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified MyCustomerDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No MyCustomerDto could be found with the provided parameters.</response>
        [HttpGet("api/my-customers/{id}")]
        [ProducesResponseType(typeof(MyCustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MyCustomerDto>> GetMyCustomerById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetMyCustomerByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;MyCustomerDto&gt;.</response>
        [HttpGet("api/my-customers")]
        [ProducesResponseType(typeof(List<MyCustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<MyCustomerDto>>> GetMyCustomers(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetMyCustomersQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
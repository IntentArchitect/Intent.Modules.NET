using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ObjectMappingTest.Application.Products;
using ObjectMappingTest.Application.Products.GetDigitalProductById;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace ObjectMappingTest.Api.Controllers
{
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ISender _mediator;

        public ProductsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified DigitalProductDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No DigitalProductDto could be found with the provided parameters.</response>
        [HttpGet("api/products/digital/{id}")]
        [ProducesResponseType(typeof(DigitalProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DigitalProductDto>> GetDigitalProductById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDigitalProductById(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
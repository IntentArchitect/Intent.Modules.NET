using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Wolverine.AspNetCore.Controllers.Application.Products;
using Wolverine.AspNetCore.Controllers.Application.Products.ChangeProductProduct;
using Wolverine.AspNetCore.Controllers.Application.Products.CreateProduct;
using Wolverine.AspNetCore.Controllers.Application.Products.GetProductById;
using Wolverine.AspNetCore.Controllers.Application.Products.GetProducts;
using Wolverine.AspNetCore.Controllers.Application.Products.UpdateProductPrice;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Api.Controllers
{
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMessageBus _sender;

        public ProductsController(IMessageBus sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/products/{id}/change-product")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangeProductProduct(
            [FromRoute] Guid id,
            [FromBody] ChangeProductProductCommand command,
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

            await _sender.InvokeAsync(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/products")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> CreateProduct(
            [FromBody] CreateProductCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.InvokeAsync<Guid>(command, cancellationToken);
            return CreatedAtAction(nameof(GetProductById), new { id = result }, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/products/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateProductPrice(
            [FromRoute] Guid id,
            [FromBody] UpdateProductPriceCommand command,
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

            await _sender.InvokeAsync(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified ProductDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No ProductDto could be found with the provided parameters.</response>
        [HttpGet("api/products/{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> GetProductById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.InvokeAsync<ProductDto>(new GetProductByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ProductDto&gt;.</response>
        [HttpGet("api/products")]
        [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ProductDto>>> GetProducts(CancellationToken cancellationToken = default)
        {
            var result = await _sender.InvokeAsync<List<ProductDto>>(new GetProductsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
using HttpQuery.Controllers.Swashbuckle.Application.Products;
using HttpQuery.Controllers.Swashbuckle.Application.Products.CreateProduct;
using HttpQuery.Controllers.Swashbuckle.Application.Products.GetProductById;
using HttpQuery.Controllers.Swashbuckle.Application.Products.SearchCategoryProducts;
using HttpQuery.Controllers.Swashbuckle.Application.Products.SearchProducts;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace HttpQuery.Controllers.Swashbuckle.Api.Controllers
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
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetProductById), new { id = result }, result);
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
            var result = await _mediator.Send(new GetProductByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// HTTP QUERY endpoint (modelled as POST until Http Settings supports QUERY). Category comes from the route; the remaining criteria from the request body.
        /// </summary>
        /// <response code="200">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [IntentManaged(Mode.Ignore)]
        [HttpQuery("api/categories/{category}/products/search")]
        [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ProductDto>>> SearchCategoryProducts(
            [FromRoute] string category,
            [FromBody] SearchCategoryProductsQuery query,
            CancellationToken cancellationToken = default)
        {
            if (category != query.Category)
            {
                return BadRequest();
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// HTTP QUERY endpoint (modelled as POST until Http Settings supports QUERY). Filters products by the criteria in the request body; every criterion is optional.
        /// </summary>
        /// <response code="200">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [IntentManaged(Mode.Ignore)]
        [HttpQuery("api/products/search")]
        [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ProductDto>>> SearchProducts(
            [FromBody] SearchProductsQuery query,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Validation;
using AdvancedMappingCrud.Cosmos.Tests.Application.Interfaces.ManyToMany;
using AdvancedMappingCrud.Cosmos.Tests.Application.ManyToMany;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Api.Controllers.ManyToMany
{
    [ApiController]
    [Route("api/product-item")]
    public class ProductItemController : ControllerBase
    {
        private readonly IProductItemService _appService;
        private readonly IValidationService _validationService;
        private readonly ICosmosDBUnitOfWork _cosmosDBUnitOfWork;

        public ProductItemController(IProductItemService appService,
            IValidationService validationService,
            ICosmosDBUnitOfWork cosmosDBUnitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _cosmosDBUnitOfWork = cosmosDBUnitOfWork ?? throw new ArgumentNullException(nameof(cosmosDBUnitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateProductItem(
            [FromBody] CreateProductItemDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            await _appService.CreateProductItem(dto, cancellationToken);
            await _cosmosDBUnitOfWork.SaveChangesAsync(cancellationToken);
            return Created(string.Empty, null);
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
        public async Task<ActionResult> UpdateProductItem(
            [FromBody] UpdateProductItemDto dto,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            await _appService.UpdateProductItem(dto, id, cancellationToken);
            await _cosmosDBUnitOfWork.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
    }
}
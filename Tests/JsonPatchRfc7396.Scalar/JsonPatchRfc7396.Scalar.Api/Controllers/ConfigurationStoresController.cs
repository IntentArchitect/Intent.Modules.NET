using System.Net.Mime;
using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Api.Controllers.ResponseTypes;
using JsonPatchRfc7396.Scalar.Api.Patching;
using JsonPatchRfc7396.Scalar.Application.Common.Validation;
using JsonPatchRfc7396.Scalar.Application.ConfigurationStores;
using JsonPatchRfc7396.Scalar.Application.Interfaces;
using JsonPatchRfc7396.Scalar.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Morcatko.AspNetCore.JsonMergePatch;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Api.Controllers
{
    [ApiController]
    [Route("api/configuration-stores")]
    public class ConfigurationStoresController : ControllerBase
    {
        private readonly IConfigurationStoresService _appService;
        private readonly IValidationService _validationService;
        private readonly IMongoDbUnitOfWork _mongoDbUnitOfWork;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidatorProvider _validatorProvider;

        public ConfigurationStoresController(IConfigurationStoresService appService,
            IValidationService validationService,
            IMongoDbUnitOfWork mongoDbUnitOfWork,
            IUnitOfWork unitOfWork,
            IValidatorProvider validatorProvider)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _mongoDbUnitOfWork = mongoDbUnitOfWork ?? throw new ArgumentNullException(nameof(mongoDbUnitOfWork));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _validatorProvider = validatorProvider ?? throw new ArgumentNullException(nameof(validatorProvider));
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
        public async Task<ActionResult<JsonResponse<Guid>>> CreateConfigurationStore(
            [FromBody] CreateConfigurationStoreDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            var result = Guid.Empty;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.CreateConfigurationStore(dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }

            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return CreatedAtAction(nameof(FindConfigurationStoreById), new { id = result }, new JsonResponse<Guid>(result));
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
        public async Task<ActionResult> UpdateConfigurationStore(
            [FromRoute] Guid id,
            [FromBody] UpdateConfigurationStoreDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.UpdateConfigurationStore(id, dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }

            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No ConfigurationStoreDto could be found with the provided parameters.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ConfigurationStoreDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Consumes(JsonMergePatchDocument.ContentType)]
        public async Task<ActionResult<ConfigurationStoreDto>> PatchConfigurationStore(
            [FromRoute] Guid id,
            [FromBody] JsonMergePatchDocument<PatchConfigurationStoreDto> mergePatchDocument,
            CancellationToken cancellationToken = default)
        {
            if (mergePatchDocument == null)
            {
                return BadRequest("Merge patch document cannot be null");
            }

            var patchExecutor = new JsonMergePatchExecutor<PatchConfigurationStoreDto>(mergePatchDocument, _validatorProvider);

            var dto = new PatchConfigurationStoreDto
            {
                PatchExecutor = patchExecutor
            };

            var result = default(ConfigurationStoreDto);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.PatchConfigurationStore(id, dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }

            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified ConfigurationStoreDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No ConfigurationStoreDto could be found with the provided parameters.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ConfigurationStoreDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ConfigurationStoreDto>> FindConfigurationStoreById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = default(ConfigurationStoreDto);
            result = await _appService.FindConfigurationStoreById(id, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ConfigurationStoreDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ConfigurationStoreDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ConfigurationStoreDto>>> FindConfigurationStores(CancellationToken cancellationToken = default)
        {
            var result = default(List<ConfigurationStoreDto>);
            result = await _appService.FindConfigurationStores(cancellationToken);
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
        public async Task<ActionResult> DeleteConfigurationStore(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.DeleteConfigurationStore(id, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }

            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No ConfigurationConfigurationItemDto could be found with the provided parameters.</response>
        [HttpPatch("{configurationStoreId}/configuration-items/{id}")]
        [ProducesResponseType(typeof(ConfigurationConfigurationItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Consumes(JsonMergePatchDocument.ContentType)]
        public async Task<ActionResult<ConfigurationConfigurationItemDto>> PatchConfigurationItem(
            [FromRoute] Guid configurationStoreId,
            [FromRoute] Guid id,
            [FromBody] JsonMergePatchDocument<PatchConfigurationItemDto> mergePatchDocument,
            CancellationToken cancellationToken = default)
        {
            if (mergePatchDocument == null)
            {
                return BadRequest("Merge patch document cannot be null");
            }

            var patchExecutor = new JsonMergePatchExecutor<PatchConfigurationItemDto>(mergePatchDocument, _validatorProvider);

            var dto = new PatchConfigurationItemDto
            {
                PatchExecutor = patchExecutor
            };

            var result = default(ConfigurationConfigurationItemDto);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.PatchConfigurationItem(configurationStoreId, id, dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }

            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
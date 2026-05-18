using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Transactions;
using AdvancedMapping.Repositories.Mapperly.Tests.Api.Controllers.ResponseTypes;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Validation;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.Interfaces;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Api.Controllers
{
    [ApiController]
    [Route("api/customer-segments")]
    public class CustomerSegmentsController : ControllerBase
    {
        private readonly ICustomerSegmentsService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerSegmentsController(ICustomerSegmentsService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified Guid.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpGet("customer-segments")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateCustomerSegments(
            [FromQuery][Required] CreateCustomerSegmentsDto dto,
            CancellationToken cancellationToken = default)
        {
            var result = Guid.Empty;
            result = await _appService.CreateCustomerSegments(dto, cancellationToken);
            return Ok(new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("customer-segments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCustomerSegments(
            Guid id,
            [FromBody] UpdateCustomerSegmentsDto dto,
            CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.UpdateCustomerSegments(id, dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified CustomerSegmentsDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No CustomerSegmentsDto could be found with the provided parameters.</response>
        [HttpGet("customer-segments/{id}")]
        [ProducesResponseType(typeof(CustomerSegmentsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerSegmentsDto>> FindCustomerSegmentsById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = default(CustomerSegmentsDto);
            result = await _appService.FindCustomerSegmentsById(id, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;CustomerSegmentsDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<CustomerSegmentsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CustomerSegmentsDto>>> FindCustomerSegments(CancellationToken cancellationToken = default)
        {
            var result = default(List<CustomerSegmentsDto>);
            result = await _appService.FindCustomerSegments(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("customer-segments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCustomerSegments(
            [FromQuery][Required] Guid id,
            CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.DeleteCustomerSegments(id, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Ok();
        }
    }
}
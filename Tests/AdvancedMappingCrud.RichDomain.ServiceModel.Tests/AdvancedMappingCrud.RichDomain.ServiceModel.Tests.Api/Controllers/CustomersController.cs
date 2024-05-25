using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Validation;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Customers;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Interfaces;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersService _appService;
        private readonly IValidationService _validationService;
        private readonly IUnitOfWork _unitOfWork;

        public CustomersController(ICustomersService appService,
            IValidationService validationService,
            IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> CreateCustomer(
            [FromBody] CustomerCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            var result = default(Guid);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.CreateCustomer(dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return CreatedAtAction(nameof(FindCustomerById), new { id = result }, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified CustomerDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No CustomerDto could be found with the provided parameters.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerDto>> FindCustomerById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = default(CustomerDto);
            result = await _appService.FindCustomerById(id, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;CustomerDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CustomerDto>>> FindCustomers(CancellationToken cancellationToken = default)
        {
            var result = default(List<CustomerDto>);
            result = await _appService.FindCustomers(cancellationToken);
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
        public async Task<ActionResult> DeleteCustomer([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.DeleteCustomer(id, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Ok();
        }
    }
}
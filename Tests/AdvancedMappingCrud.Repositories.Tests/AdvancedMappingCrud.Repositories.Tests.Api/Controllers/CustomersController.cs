using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Api.Controllers.ResponseTypes;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Pagination;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.ApproveQuote;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.CreateCorporateFuneralCoverQuote;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.CreateCustomer;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.CreateFuneralCoverQuote;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.CreateQuote;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.DeactivateCustomer;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.DeleteCustomer;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomerById;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomers;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomersByNameAndSurname;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomersPaginated;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomersPaginatedWithOrder;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomerStatistics;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomersWithParams;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.UpdateCorporateFuneralCoverQuote;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers.UpdateCustomer;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Validator;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Api.Controllers
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
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("api/customers/{quoteId}/approve")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApproveQuote(
            [FromRoute] Guid quoteId,
            [FromBody] ApproveQuoteCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.QuoteId == default)
            {
                command.QuoteId = quoteId;
            }

            if (quoteId != command.QuoteId)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/customers/funeralquote")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateCorporateFuneralCoverQuote(
            [FromBody] CreateCorporateFuneralCoverQuoteCommand command,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/customer")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateCustomer(
            [FromBody] CreateCustomerCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetCustomerById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/customers/funeral")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateFuneralCoverQuote(
            [FromBody] CreateFuneralCoverQuoteCommand command,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/customers")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateQuote(
            [FromBody] CreateQuoteCommand command,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("api/customers/deactivate-customer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeactivateCustomer(
            [FromBody] DeactivateCustomerCommand command,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/customer/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCustomer([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteCustomerCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/customers/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCorporateFuneralCoverQuote(
            [FromRoute] Guid id,
            [FromBody] UpdateCorporateFuneralCoverQuoteCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == default)
            {
                command.Id = id;
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
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/customer/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCustomer(
            [FromRoute] Guid id,
            [FromBody] UpdateCustomerCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == default)
            {
                command.Id = id;
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
        /// <response code="200">Returns the specified CustomerDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No CustomerDto could be found with the provided parameters.</response>
        [HttpGet("api/customer/{id}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerDto>> GetCustomerById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomerByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;CustomerDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/customer/{name}/{surname}")]
        [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CustomerDto>>> GetCustomersByNameAndSurname(
            [FromRoute] string name,
            [FromRoute] string surname,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomersByNameAndSurnameQuery(name: name, surname: surname), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;CustomerDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/customer/{name}/{surname}/{isActive}/paged")]
        [ProducesResponseType(typeof(PagedResult<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<CustomerDto>>> GetCustomersPaginated(
            [FromRoute] bool isActive,
            [FromRoute] string name,
            [FromRoute] string surname,
            [FromQuery] int pageNo,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomersPaginatedQuery(isActive: isActive, name: name, surname: surname, pageNo: pageNo, pageSize: pageSize), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;CustomerDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/customer/{name}/{surname}/{isActive}/paged/ordered")]
        [ProducesResponseType(typeof(PagedResult<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<CustomerDto>>> GetCustomersPaginatedWithOrder(
            [FromRoute] bool isActive,
            [FromRoute] string name,
            [FromRoute] string surname,
            [FromQuery] int pageNo,
            [FromQuery] int pageSize,
            [FromQuery] string orderBy,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomersPaginatedWithOrderQuery(isActive: isActive, name: name, surname: surname, pageNo: pageNo, pageSize: pageSize, orderBy: orderBy), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;CustomerDto&gt;.</response>
        [HttpGet("api/customers")]
        [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CustomerDto>>> GetCustomers(
            ODataQueryOptions<CustomerDto> oDataOptions,
            CancellationToken cancellationToken = default)
        {
            ValidateODataOptions(oDataOptions);

            var result = await _mediator.Send(new GetCustomersQuery(oDataOptions.ApplyTo), cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified int.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpGet("api/customers/statistics")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetCustomerStatistics(
            [FromQuery] Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomerStatisticsQuery(customerId: customerId), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;CustomerDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/customer/{name}/{surname}/{isActive}")]
        [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CustomerDto>>> GetCustomersWithParams(
            [FromRoute] bool isActive,
            [FromRoute] string? name,
            [FromRoute] string? surname,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomersWithParamsQuery(isActive: isActive, name: name, surname: surname), cancellationToken);
            return Ok(result);
        }

        private void ValidateODataOptions<TDto>(ODataQueryOptions<TDto> options, bool enableSelect = false)
        {
            var settings = new ODataValidationSettings();

            if (!enableSelect)
            {
                settings.AllowedQueryOptions = AllowedQueryOptions.All & ~AllowedQueryOptions.Select;
            }
            options.Validate(settings);
        }
    }
}
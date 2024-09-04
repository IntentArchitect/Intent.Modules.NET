using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using CleanArchitecture.Comprehensive.Application.Pagination;
using CleanArchitecture.Comprehensive.Application.Pagination.GetLogEntries;
using CleanArchitecture.Comprehensive.Application.Pagination.GetPeopleByFirstNamePaginated;
using CleanArchitecture.Comprehensive.Application.Pagination.GetPeopleByNullableFirstNamePaginated;
using CleanArchitecture.Comprehensive.Application.Pagination.GetPeoplePaginated;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.Controllers
{
    [ApiController]
    public class PaginationController : ControllerBase
    {
        private readonly ISender _mediator;

        public PaginationController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;LogEntryDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/pagination/log-entries")]
        [ProducesResponseType(typeof(PagedResult<LogEntryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<LogEntryDto>>> GetLogEntries(
            [FromQuery] int pageNo,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetLogEntriesQuery(pageNo: pageNo, pageSize: pageSize), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;PersonEntryDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/pagination/firstname/{firstName}")]
        [ProducesResponseType(typeof(PagedResult<PersonEntryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<PersonEntryDto>>> GetPeopleByFirstNamePaginated(
            [FromRoute] string firstName,
            [FromQuery] int pageNo,
            [FromQuery] int pageSize,
            [FromQuery] string? orderBy,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPeopleByFirstNamePaginatedQuery(firstName: firstName, pageNo: pageNo, pageSize: pageSize, orderBy: orderBy), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;PersonEntryDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/pagination/nullable-firstname/{firstName}")]
        [ProducesResponseType(typeof(PagedResult<PersonEntryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<PersonEntryDto>>> GetPeopleByNullableFirstNamePaginated(
            [FromRoute] string? firstName,
            [FromQuery] int pageNo,
            [FromQuery] int pageSize,
            [FromQuery] string? orderBy,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPeopleByNullableFirstNamePaginatedQuery(firstName: firstName, pageNo: pageNo, pageSize: pageSize, orderBy: orderBy), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;PersonEntryDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/pagination")]
        [ProducesResponseType(typeof(PagedResult<PersonEntryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<PersonEntryDto>>> GetPeoplePaginated(
            [FromQuery] int pageNo,
            [FromQuery] int pageSize,
            [FromQuery] string? orderBy,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPeoplePaginatedQuery(pageNo: pageNo, pageSize: pageSize, orderBy: orderBy), cancellationToken);
            return Ok(result);
        }
    }
}
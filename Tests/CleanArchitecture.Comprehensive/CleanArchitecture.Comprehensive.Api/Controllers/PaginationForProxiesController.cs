using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using CleanArchitecture.Comprehensive.Application.PaginationForProxies.PaginatedResult;
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
    public class PaginationForProxiesController : ControllerBase
    {
        private readonly ISender _mediator;

        public PaginationForProxiesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified PagedResult&lt;string&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("api/pagination-for-proxies/paginated-result")]
        [ProducesResponseType(typeof(PagedResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<string>>> PaginatedResult(
            [FromQuery] int pageNo,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new PaginatedResultQuery(pageNo: pageNo, pageSize: pageSize), cancellationToken);
            return Ok(result);
        }
    }
}
using CqrsSpike.Publisher.Application.AuditEntries.GetAuditEntries;
using CqrsSpike.Publisher.Application.AuditEntries;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CqrsSpike.Publisher.Api.Controllers
{
    [ApiController]
    public class AuditEntriesController : ControllerBase
    {
        private readonly ISender _mediator;

        public AuditEntriesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;AuditEntryDto&gt;.</response>
        [HttpGet("api/audit-entries")]
        [ProducesResponseType(typeof(List<AuditEntryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AuditEntryDto>>> GetAuditEntries(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAuditEntriesQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
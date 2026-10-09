using CqrsSpike.Publisher.Application.AuditEntries;
using CqrsSpike.Publisher.Application.AuditEntries.GetAuditEntries;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

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
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using WolverineEventing.Subscribe.RabbitMQ.Application;
using WolverineEventing.Subscribe.RabbitMQ.Application.GetShippedOrderRecords;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Api.Controllers
{
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly IMessageBus _sender;

        public DefaultController(IMessageBus sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;ShippedOrderRecordDto&gt;.</response>
        [HttpGet("api/shipped-order-records")]
        [ProducesResponseType(typeof(List<ShippedOrderRecordDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ShippedOrderRecordDto>>> GetShippedOrderRecords(CancellationToken cancellationToken = default)
        {
            var result = await _sender.InvokeAsync<List<ShippedOrderRecordDto>>(new GetShippedOrderRecordsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
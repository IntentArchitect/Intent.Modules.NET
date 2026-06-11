using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NServiceBus.RabbitMQ.Application.ExternalMessagePublish.PublishExternalMessage;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace NServiceBus.RabbitMQ.Api.Controllers
{
    [ApiController]
    public class ExternalMessagePublishController : ControllerBase
    {
        private readonly ISender _mediator;

        public ExternalMessagePublishController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        [HttpPut("api/external-message-publish/publish-external-message")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PublishExternalMessage(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new PublishExternalMessageCommand(), cancellationToken);
            return NoContent();
        }
    }
}
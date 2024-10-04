using System.Threading;
using System.Threading.Tasks;
using Dapr;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Publish.CleanArchDapr.TestApplication.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Pubsub.DaprEventHandlerController", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Api.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class DaprEventHandlerController : ControllerBase
    {
        private readonly ISender _mediatr;

        public DaprEventHandlerController(ISender mediatr)
        {
            _mediatr = mediatr;
        }

        [HttpPost]
        [Topic(CustomerCreatedEvent.PubsubName, CustomerCreatedEvent.TopicName)]
        public async Task HandleCustomerCreatedEvent(CustomerCreatedEvent @event, CancellationToken cancellationToken)
        {
            await _mediatr.Send(@event, cancellationToken);
        }

        [HttpPost]
        [Topic(OrderCreatedEvent.PubsubName, OrderCreatedEvent.TopicName)]
        public async Task HandleOrderCreatedEvent(OrderCreatedEvent @event, CancellationToken cancellationToken)
        {
            await _mediatr.Send(@event, cancellationToken);
        }
    }
}
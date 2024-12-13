using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Publish.CleanArchDapr.TestApplication.Eventing.Messages;
using Subscribe.CleanArchDapr.TestApplication.Application.Common.Eventing;

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
        private readonly IServiceProvider _serviceProvider;

        public DaprEventHandlerController(ISender mediatr, IServiceProvider serviceProvider)
        {
            _mediatr = mediatr;
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        [Topic(CustomerCreatedEvent.PubsubName, CustomerCreatedEvent.TopicName)]
        public async Task HandleCustomerCreatedEvent(CustomerCreatedEvent @event, CancellationToken cancellationToken)
        {
            var handler = _serviceProvider.GetRequiredService<IIntegrationEventHandler<CustomerCreatedEvent>>();
            await handler.HandleAsync(@event, cancellationToken);
        }

        [HttpPost]
        [Topic(OrderCreatedEvent.PubsubName, OrderCreatedEvent.TopicName)]
        public async Task HandleOrderCreatedEvent(OrderCreatedEvent @event, CancellationToken cancellationToken)
        {
            var handler = _serviceProvider.GetRequiredService<IIntegrationEventHandler<OrderCreatedEvent>>();
            await handler.HandleAsync(@event, cancellationToken);
        }
    }
}
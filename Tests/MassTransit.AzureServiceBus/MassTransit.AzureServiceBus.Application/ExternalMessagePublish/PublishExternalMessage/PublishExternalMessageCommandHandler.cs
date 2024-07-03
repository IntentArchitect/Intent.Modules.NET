using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.Common.Eventing;
using MassTransit.AzureServiceBus.Services.External;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.ExternalMessagePublish.PublishExternalMessage
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PublishExternalMessageCommandHandler : IRequestHandler<PublishExternalMessageCommand>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public PublishExternalMessageCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(PublishExternalMessageCommand request, CancellationToken cancellationToken)
        {
            _eventBus.Publish(new MessageWithTopologyEvent
            {
            });
        }
    }
}
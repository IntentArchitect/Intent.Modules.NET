using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureEventGrid.Application.Common.Eventing;
using AzureFunctions.AzureEventGrid.GroupA.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.AzureEventGrid.Application.CreateClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreateClientCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            //IntentIgnore
            _eventBus.Publish(new ClientCreatedEvent
            {
                Name = request.Name
            },
            new Dictionary<string, object>
            {
                ["a"] = "b"
            });
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.Common.Eventing;
using MassTransit.RabbitMQ.Services;
using MassTransit.RabbitMQ.Services.Animals;
using MassTransit.RabbitMQ.Services.People;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandler", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CatchAllHandler : IIntegrationEventHandler<OrderAnimal>, IIntegrationEventHandler<MakeSoundCommand>, IIntegrationEventHandler<CreatePersonIdentity>, IIntegrationEventHandler<TalkToPersonCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CatchAllHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(OrderAnimal message, CancellationToken cancellationToken = default)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(MakeSoundCommand message, CancellationToken cancellationToken = default)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(CreatePersonIdentity message, CancellationToken cancellationToken = default)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(TalkToPersonCommand message, CancellationToken cancellationToken = default)
        {
        }
    }
}
using Intent.RoslynWeaver.Attributes;
using NServiceBus.SQS.Application.Common.Eventing;
using NServiceBus.SQS.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace NServiceBus.SQS.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CatchAllHandler : IIntegrationEventHandler<OrderAnimal>, IIntegrationEventHandler<MakeSoundCommand>, IIntegrationEventHandler<TalkToPersonCommand>, IIntegrationEventHandler<CreatePersonIdentity>
    {
        [IntentManaged(Mode.Merge)]
        public CatchAllHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(OrderAnimal message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[HANDLER HIT] SQS.CatchAllHandler received OrderAnimal: Name={message.Name}, Type={message.Type}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(MakeSoundCommand message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[HANDLER HIT] SQS.CatchAllHandler received MakeSoundCommand: Name={message.Name}, Type={message.Type}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(TalkToPersonCommand message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[HANDLER HIT] SQS.CatchAllHandler received TalkToPersonCommand");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(CreatePersonIdentity message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[HANDLER HIT] SQS.CatchAllHandler received CreatePersonIdentity");
        }
    }
}
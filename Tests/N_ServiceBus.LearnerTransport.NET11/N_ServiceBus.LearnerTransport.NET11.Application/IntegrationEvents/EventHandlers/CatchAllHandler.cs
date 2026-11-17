using Intent.RoslynWeaver.Attributes;
using N_ServiceBus.LearnerTransport.NET11.Application.Common.Eventing;
using N_ServiceBus.LearnerTransport.NET11.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace N_ServiceBus.LearnerTransport.NET11.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CatchAllHandler : IIntegrationEventHandler<OrderAnimal>, IIntegrationEventHandler<MakeSoundCommand>, IIntegrationEventHandler<TalkToPersonCommand>, IIntegrationEventHandler<CreatePersonIdentity>
    {
        [IntentManaged(Mode.Merge)]
        public CatchAllHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(OrderAnimal message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[HANDLER HIT] CatchAllHandler received: {message.GetType().Name}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(MakeSoundCommand message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[HANDLER HIT] CatchAllHandler received: {message.GetType().Name}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(TalkToPersonCommand message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[HANDLER HIT] CatchAllHandler received: {message.GetType().Name}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(CreatePersonIdentity message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[HANDLER HIT] CatchAllHandler received: {message.GetType().Name}");
        }
    }
}
using Intent.RoslynWeaver.Attributes;
using N_ServiceBus.Persistence.Sql.Publish.Eventing.Messages;
using N_ServiceBus.Persistence.Sql.Subscribe.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace N_ServiceBus.Persistence.Sql.Subscribe.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestCommandHandler : IIntegrationEventHandler<TestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public TestCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(TestCommand message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[HANDLER HIT] Subscribe.TestCommandHandler received TestCommand");
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Solace.Tests.Application.Common.Eventing;
using Solace.Tests.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.IntegrationEventHandler", Version = "1.0")]

namespace Solace.Tests.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GeneralHandler : IIntegrationEventHandler<AccountCreatedEvent>, IIntegrationEventHandler<PurchaseCreated>
    {
        [IntentManaged(Mode.Merge)]
        public GeneralHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(AccountCreatedEvent message, CancellationToken cancellationToken = default)
        {
            //IntentIgnore
            Console.WriteLine("Received:AccountCreatedEvent");

        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(PurchaseCreated message, CancellationToken cancellationToken = default)
        {
            //IntentIgnore
            Console.WriteLine("Received:PurchaseCreated");

        }
    }
}
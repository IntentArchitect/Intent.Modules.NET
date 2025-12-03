using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArchDapr.TestApplication.Application.Common.Eventing;
using Publish.CleanArchDapr.TestApplication.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Application.IntegrationEventHandlers.Customers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class FullNamespaceEventHandler : IIntegrationEventHandler<FullNamespaceEvent>
    {
        [IntentManaged(Mode.Merge)]
        public FullNamespaceEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(FullNamespaceEvent message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (FullNamespaceEventHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandlerInterface", Version = "1.0")]

namespace Publish.AspNetCore.GooglePubSub.TestApplication.Application.Common.Eventing
{
    public interface IIntegrationEventHandler<TMessage>
        where TMessage : class
    {
        Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
    }
}
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandlerInterface", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Application.Core.Common.Eventing
{
    public interface IIntegrationEventHandler<in TMessage>
        where TMessage : class
    {
        Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
    }
}
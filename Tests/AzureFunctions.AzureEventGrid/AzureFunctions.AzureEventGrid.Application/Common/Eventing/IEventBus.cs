using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.EventBusInterface", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Application.Common.Eventing
{
    public interface IEventBus
    {
        void Publish<T>(T message)
            where T : class;
        void Publish<T>(T message, string subject)
            where T : class;
        Task FlushAllAsync(CancellationToken cancellationToken = default);
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.EventBusInterface", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Application.Common.Eventing
{
    public interface IEventBus
    {
        void Publish<T>(T message)
            where T : class, IEvent;
        Task FlushAllAsync(CancellationToken cancellationToken = default);
    }
}
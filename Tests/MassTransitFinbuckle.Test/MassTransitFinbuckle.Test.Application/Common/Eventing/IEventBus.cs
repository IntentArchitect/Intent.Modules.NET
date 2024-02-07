using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.EventBusInterface", Version = "1.0")]

namespace MassTransitFinbuckle.Test.Application.Common.Eventing
{
    public interface IEventBus
    {
        void Publish<T>(T message)
            where T : class;
        Task FlushAllAsync(CancellationToken cancellationToken = default);
        void Send<T>(T message)
            where T : class;
        void Send<T>(T message, Uri address)
            where T : class;
    }
}
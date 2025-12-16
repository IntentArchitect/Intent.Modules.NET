using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.EventBusInterface", Version = "1.0")]

namespace Solace.Tests.Application.Common.Eventing
{
    public interface IEventBus
    {
        void Publish<TMessage>(TMessage message)
            where TMessage : class;
        Task FlushAllAsync(CancellationToken cancellationToken = default);
        void Send<TMessage>(TMessage message)
            where TMessage : class;
    }
}
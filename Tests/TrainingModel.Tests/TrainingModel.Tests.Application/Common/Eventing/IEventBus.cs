using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.EventBusInterface", Version = "1.0")]

namespace TrainingModel.Tests.Application.Common.Eventing
{
    public interface IEventBus
    {
        void Publish<TMessage>(TMessage message)
            where TMessage : class;
        void Publish<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class;
        void Send<TMessage>(TMessage message)
            where TMessage : class;
        void Send<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class;
        Task FlushAllAsync(CancellationToken cancellationToken = default);
    }
}
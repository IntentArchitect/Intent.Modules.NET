using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application.Interfaces.NamingOverrides
{
    public interface IInvokedFromEventHandlerService : IDisposable
    {
        Task Operation(string message, CancellationToken cancellationToken = default);
    }
}
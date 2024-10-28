using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace FastEndpointsTest.Application.Interfaces.Headers
{
    public interface IServiceWithHeaderFieldService : IDisposable
    {
        Task DoHeaderField(string param, CancellationToken cancellationToken = default);
    }
}
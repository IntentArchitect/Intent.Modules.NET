using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Integration.HttpClients.TestApplication.Application.Interfaces
{
    public interface IVersionOneService : IDisposable
    {
        Task OperationForVersionOne(string param, CancellationToken cancellationToken = default);
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace Standard.AspNetCore.TestApplication.Application.IntegrationServices
{
    public interface IQueryStringNamesService : IDisposable
    {
        Task OperationAsync(string par1, CancellationToken cancellationToken = default);
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Interfaces
{
    public interface IOpenApiIgnoreAllImplicitService : IDisposable
    {
        Task OperationA(CancellationToken cancellationToken = default);
        Task OperationB(CancellationToken cancellationToken = default);
    }
}
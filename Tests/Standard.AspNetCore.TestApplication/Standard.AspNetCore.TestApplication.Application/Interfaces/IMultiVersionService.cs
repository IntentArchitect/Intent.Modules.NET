using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Interfaces
{
    public interface IMultiVersionService : IDisposable
    {
        Task OperationForVersionOne(CancellationToken cancellationToken = default);
        Task OperationForVersionTwo(CancellationToken cancellationToken = default);
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Application.Interfaces
{
    public interface IAuditedService : IDisposable
    {
        Task Create(CancellationToken cancellationToken = default);
        Task Update(Guid id, CancellationToken cancellationToken = default);
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.MappableStoredProcs
{
    public interface IMappableStoredProcRepository
    {
        Task<EntityRecord> GetEntityById(Guid id, CancellationToken cancellationToken = default);
        Task CreateEntities(IEnumerable<EntityRecord> entities, CancellationToken cancellationToken = default);
        Task<string> GetEntityName(Guid id, CancellationToken cancellationToken = default);
        Task DoSomething(CancellationToken cancellationToken = default);
        Task CreateEntity(Guid id, string name, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<EntityRecord>> GetEntities(CancellationToken cancellationToken = default);
        Task<MappedSpResult> MappedOperation(string paramName2WithAltName, string paramSomething1WithAltName, CancellationToken cancellationToken = default);
        Task<MappedSpResultCollection> MappedOperationWithCollection(string paramRandom2WithAltName, string paramElse1WithAltName, CancellationToken cancellationToken = default);
        Task<int> MappedOperationCallingScalar(int param1WithDiffName, CancellationToken cancellationToken = default);
    }
}
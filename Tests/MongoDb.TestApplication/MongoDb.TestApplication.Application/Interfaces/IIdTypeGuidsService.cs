using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdTypeGuids;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{

    public interface IIdTypeGuidsService : IDisposable
    {
        Task<Guid> CreateIdTypeGuid(IdTypeGuidCreateDto dto, CancellationToken cancellationToken = default);
        Task<IdTypeGuidDto> FindIdTypeGuidById(Guid id, CancellationToken cancellationToken = default);
        Task<List<IdTypeGuidDto>> FindIdTypeGuids(CancellationToken cancellationToken = default);
        Task UpdateIdTypeGuid(Guid id, IdTypeGuidUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteIdTypeGuid(Guid id, CancellationToken cancellationToken = default);

    }
}